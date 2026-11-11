#!/usr/bin/env python3
"""Load a small set of PUBLIC sample data into a DAIKON server, one or a few records per stage, for testing.

===SETUP===
1. In the browser console on dev, run this to copy your token to the clipboard:
copy(JSON.parse(localStorage[Object.keys(localStorage).find(k => k.startsWith("oidc.user:"))]).access_token)
2. In the terminal:
export DAIKON_TOKEN="$(pbpaste)"
python3 src/load_sample/load_sample.py

===========

Strain -> genes -> targets -> screens (target-based and phenotypic) -> hit collections and hits (registers the
molecules in MolecuLogix) -> hit assessments -> projects (portfolio and post-portfolio).

Everything here is public: M. tuberculosis H37Rv genes (Mycobrowser loci, UniProt accessions) and reference
compounds with their ChEMBL structures. Activity values are illustrative, and every record says it is sample data.
Never load confidential data from another environment with this.

    cp load_sample.conf.example load_sample.conf      # then set url (gitignored)
    export DAIKON_TOKEN=...                            # the signed-in user's access token for that server
    python3 load_sample.py [--conf load_sample.conf] [--yes]

Getting a token: sign in to the frontend, open the browser console and run
    copy(JSON.parse(localStorage[Object.keys(localStorage).find(k => k.startsWith("oidc.user:"))]).access_token)
then `export DAIKON_TOKEN="$(pbpaste)"`. The token expires after about an hour.

Re-running is safe: a record whose name already exists is reused, not created again. Records are created as the
token's user, in that user's organization unless the conf names another.
"""
import argparse, configparser, json, os, sys, time, urllib.error, urllib.request
from pathlib import Path

NOTE = "Sample data for testing (public sources; activity values illustrative only)."

# (locus, gene name, UniProt, product, functional category)
GENES = [
    ("Rv1484", "inhA", "P9WGR1", "Enoyl-[acyl-carrier-protein] reductase [NADH] InhA", "lipid metabolism"),
    ("Rv3790", "dprE1", "P9WJF1", "Decaprenylphosphoryl-beta-D-ribose oxidase DprE1", "cell wall and cell processes"),
    ("Rv0206c", "mmpL3", "P9WJV5", "Trehalose monomycolate exporter MmpL3", "cell wall and cell processes"),
    ("Rv1305", "atpE", "P9WPS1", "ATP synthase subunit c AtpE", "intermediary metabolism and respiration"),
    ("Rv0005", "gyrB", "P9WG45", "DNA gyrase subunit B GyrB", "information pathways"),
    ("Rv3795", "embB", "P9WNL7", "Arabinosyltransferase B EmbB", "cell wall and cell processes"),
]

# (target name, gene locus, bucket, impact score, likelihood score, background)
TARGETS = [
    ("InhA", "Rv1484", "1", 0.9, 0.8, "Activated isoniazid and ethionamide inhibit it; triclosan analogues are direct inhibitors."),
    ("DprE1", "Rv3790", "1", 0.85, 0.75, "Target of the benzothiazinones (BTZ043, macozinone/PBTZ169) in clinical development."),
    ("MmpL3", "Rv0206c", "2", 0.7, 0.6, "Mycolic acid transporter; proposed target of SQ109 and several phenotypic series."),
    ("AtpE", "Rv1305", "2", 0.8, 0.5, "ATP synthase c subunit; target of bedaquiline."),
]

# compound name -> (ChEMBL SMILES, IC50 µM or None, MIC µg/mL)
COMPOUNDS = {
    "Triclosan": ("Oc1cc(Cl)ccc1Oc1ccc(Cl)cc1Cl", "1.0", "12.5"),
    "Ethionamide": ("CCc1cc(C(N)=S)ccn1", "50", "0.6"),
    "Isoniazid": ("NNC(=O)c1ccncc1", ">100", "0.05"),
    "Macozinone": ("O=c1nc(N2CCN(CC3CCCCC3)CC2)sc2c([N+](=O)[O-])cc(C(F)(F)F)cc12", "0.01", "0.0003"),
    "Bedaquiline": ("COc1nc2ccc(Br)cc2cc1[C@@H](c1ccccc1)[C@@](O)(CCN(C)C)c1cccc2ccccc12", None, "0.06"),
    "Pretomanid": ("O=[N+]([O-])c1cn2c(n1)OC[C@@H](OCc1ccc(OC(F)(F)F)cc1)C2", None, "0.25"),
    "SQ109": ("CC(C)=CCC/C(C)=C/CNCCNC1C2CC3CC(C2)CC1C3", None, "0.5"),
}

# (screen name, type, associated target or None, method, status, hit compounds)
SCREENS = [
    ("InhA-Sample-1", "target-based", "InhA", "biochemical", "Completed", ["Triclosan", "Ethionamide", "Isoniazid"]),
    ("DprE1-Sample-1", "target-based", "DprE1", "biochemical", "Ongoing", ["Macozinone"]),
    ("Sample-Phenotypic-WholeCell", "phenotypic", None, "other", "Completed", ["Bedaquiline", "Pretomanid", "SQ109"]),
]

# (hit assessment name, screen, compound, status, completed successfully and promoted)
HIT_ASSESSMENTS = [
    ("Triclosan-Sample", "InhA-Sample-1", "Triclosan", "Active", False),
    ("Macozinone-Sample", "DprE1-Sample-1", "Macozinone", "CompleteSuccess", True),
    ("SQ109-Sample", "Sample-Phenotypic-WholeCell", "SQ109", "ReadyForHA", False),
    ("Bedaquiline-Sample", "Sample-Phenotypic-WholeCell", "Bedaquiline", "CompleteSuccess", True),
]

# (project name, alias, stage, from hit assessment); H2L/LO/SP show under Portfolio, IND/P1 under Post-Portfolio
PROJECTS = [
    ("Macozinone-Sample", "DMOZ", "H2L", "Macozinone-Sample"),
    ("Bedaquiline-Sample", "DBDQ", "IND", "Bedaquiline-Sample"),
]


class Api:
    def __init__(self, url, token):
        self.base, self.token = url.rstrip("/") + "/v2/", token

    def __call__(self, method, path, body=None):
        req = urllib.request.Request(self.base + path, method=method, data=None if body is None else json.dumps(body).encode(),
                                     headers={"Authorization": f"Bearer {self.token}", "Content-Type": "application/json",
                                              "Accept": "application/json",
                                              # Cloudflare's browser integrity check rejects urllib's default agent (error 1010)
                                              "User-Agent": "daikon-load-sample/1.0"})
        try:
            with urllib.request.urlopen(req, timeout=60) as r:
                text = r.read().decode()
        except urllib.error.HTTPError as e:
            sys.exit(f"{method} {path}: HTTP {e.code} {e.read().decode()[:500]}")
        return json.loads(text) if text else None


def wait_for(fetch, what, timeout=60):
    """Reads are projected asynchronously (Kafka), so poll until the record shows up."""
    deadline = time.time() + timeout
    while (found := fetch()) is None:
        if time.time() > deadline:
            sys.exit(f"timed out waiting for {what} to appear")
        time.sleep(2)
    return found


def ensure(api, list_path, key, value, create_path, body, label):
    """Return the id of the record whose `key` equals `value`, creating it first if there is none."""
    find = lambda: next((x["id"] for x in api("GET", list_path) or [] if x.get(key) == value), None)
    if (rid := find()) is not None:
        print(f"  = {label} {value} (exists)")
        return rid
    api("POST", create_path, body)
    rid = wait_for(find, f"{label} {value}")
    print(f"  + {label} {value}")
    return rid


def load(api, strain_name, org):
    print("Strain")
    strain = ensure(api, "strain", "name", strain_name, "strain",
                    {"name": strain_name, "organism": "Mycobacterium tuberculosis"}, "strain")

    print("Genes")
    genes = {acc: ensure(api, "gene", "accessionNumber", acc, "gene",
                         {"strainId": strain, "accessionNumber": acc, "name": name, "uniProtKB": uni, "product": product,
                          "functionalCategory": cat, "comments": NOTE + " Mycobrowser/UniProt annotation."}, "gene")
             for acc, name, uni, product, cat in GENES}

    print("Targets")
    targets = {name: ensure(api, "target", "name", name, "target",
                            {"strainId": strain, "name": name, "associatedGenes": {genes[acc]: acc}, "targetType": "protein",
                             "bucket": bucket, "impactScore": impact, "likeScore": like, "background": f"{NOTE} {bg}"}, "target")
               for name, acc, bucket, impact, like, bg in TARGETS}

    print("Screens, hit collections and hits")
    hits = {}  # (screen, compound) -> hit with moleculeId
    for name, kind, target, method, status, compounds in SCREENS:
        screen = ensure(api, "screen", "name", name, "screen",
                        {"strainId": strain, "name": name, "screenType": kind, "method": method, "status": status, "notes": NOTE,
                         "associatedTargets": {targets[target]: target} if target else {},
                         "primaryOrgId": org["id"], "primaryOrgName": org["name"], "participatingOrgs": {}}, "screen")
        hc_name = f"{name} Confirmed Hits"
        hc = ensure(api, f"hit-collection/by-screen/{screen}", "name", hc_name, "hit-collection",
                    {"screenId": screen, "name": hc_name, "hitCollectionType": "validated", "notes": NOTE}, "hit collection")
        existing = {h.get("requestedSMILES") for h in api("GET", f"hit-collection/{hc}").get("hits") or []}
        new = [c for c in compounds if COMPOUNDS[c][0] not in existing]
        if new:
            api("POST", f"hit-collection/{hc}/hit/batch", [hit_body(c) for c in new])
        by_smiles = wait_for(lambda: (lambda hs: hs if all(COMPOUNDS[c][0] in hs for c in compounds) else None)(
            {h.get("requestedSMILES"): h for h in api("GET", f"hit-collection/{hc}").get("hits") or [] if h.get("moleculeId")}),
            f"hits of {hc_name}")
        for c in compounds:
            hits[name, c] = by_smiles[COMPOUNDS[c][0]]
        print(f"  {'+' if new else '='} hits {', '.join(compounds)}" + (f" (added {', '.join(new)})" if new and new != compounds else ""))

    print("Hit assessments")
    has = {}
    for name, screen, compound, status, success in HIT_ASSESSMENTS:
        hit, (smiles, ic50, mic) = hits[screen, compound], COMPOUNDS[compound]
        flags = {"isHAComplete": True, "isHASuccess": True, "isHAPromoted": True} if success else {}
        has[name] = (ensure(api, "hit-assessment", "name", name, "hit-assessment",
                            {"strainId": strain, "name": name, "status": status, "description": NOTE,
                             "hitCollectionId": hit["hitCollectionId"], "hitId": hit["id"], "compoundId": hit["moleculeId"],
                             "compoundSMILES": smiles, "compoundMIC": mic, "compoundIC50": ic50 or "0",
                             "associatedHitIds": {hit["moleculeId"]: hit["id"]},
                             "primaryOrgId": org["id"], "participatingOrgs": [], **flags}, "hit assessment"), hit)

    print("Projects")
    for name, alias, stage, ha_name in PROJECTS:
        ha, hit = has[ha_name]
        ensure(api, "project", "name", name, "project",
               {"strainId": strain, "name": name, "alias": alias, "stage": stage, "description": NOTE,
                "haId": ha, "compoundId": hit["moleculeId"], "compoundSMILES": hit["requestedSMILES"],
                "hitCompoundId": hit["moleculeId"], "hitId": hit["id"], "priority": "High", "probability": "Medium",
                "primaryOrgId": org["id"], "primaryOrgName": org["name"], "participatingOrgs": []}, "project")


def hit_body(compound):
    smiles, ic50, mic = COMPOUNDS[compound]
    return {"moleculeName": compound, "requestedSMILES": smiles, "library": "Public reference compounds",
            "librarySource": "ChEMBL", "mic": mic, "micUnit": "µg/mL", "micCondition": "7H9/ADC/Tween (illustrative)",
            **({"iC50": ic50, "iC50Unit": "µM"} if ic50 else {}), "notes": NOTE}


def main():
    here = Path(__file__).resolve().parent
    p = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    p.add_argument("--conf", default=here / "load_sample.conf", type=Path)
    p.add_argument("--yes", action="store_true", help="do not ask before writing")
    args = p.parse_args()

    if not args.conf.exists():
        sys.exit(f"{args.conf} not found: copy load_sample.conf.example to it and set url")
    conf = configparser.ConfigParser()
    conf.read(args.conf)
    url = conf.get("server", "url", fallback="").strip()
    if not url or "example" in url:
        sys.exit(f"{args.conf}: set [server] url to the gateway base, e.g. https://<host>/simplegw")
    token = os.environ.get("DAIKON_TOKEN", "").strip()
    if not token:
        sys.exit("DAIKON_TOKEN is not set (see the top of this script for how to get one)")

    api = Api(url, token)
    me = api("GET", "user/header")
    org_name = conf.get("data", "org", fallback="").strip()
    if org_name:
        org = next((o for o in api("GET", "org") if org_name in (o["name"], o.get("alias"))), None) or sys.exit(f"no org named {org_name}")
    else:
        org = me["appOrg"]
    strain = conf.get("data", "strain", fallback="H37Rv").strip()

    print(f"Server: {url}\nUser:   {me['email']}\nOrg:    {org['name']}\nStrain: {strain}")
    if not args.yes and input("Load the public sample data into this server? [y/N] ").strip().lower() != "y":
        sys.exit("aborted")
    load(api, strain, org)
    print("Done.")


if __name__ == "__main__":
    main()
