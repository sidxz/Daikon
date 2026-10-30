# Security Policy

## Supported versions

The `main` branch is the supported version and receives every fix. Older release branches are not patched; upgrade to the current release.

## Reporting a vulnerability

Please do not open a public issue for a security problem.

Report it privately through GitHub: https://github.com/sidxz/Daikon/security/advisories/new

Include the affected service (the gateway, a microservice, or the deployment), steps to reproduce, and the impact you observed. You will receive an acknowledgement within five working days, updates while a fix is prepared, and credit in the release notes unless you ask otherwise.

## Scope

DAIKON's backend sits behind its API gateway and an institutional identity provider (Microsoft Entra ID, or KeyCloak). Every microservice validates the bearer token itself and resolves the caller from it; identity is never taken from request headers. Molecule records live in the separate chemvault service; issues there belong to that project.

## Automated checks

- Every pull request and every push to `main` runs, in `.github/workflows/ci.yml`:
  - the auth, identity, header and hit-batch tests;
  - NuGet's vulnerability audit, with high and critical findings as errors;
  - `src/check-config-hygiene.sh`, which refuses credentials or environment-specific configuration in tracked files;
  - gitleaks on the new commits.
- Every service image is then built on freshly pulled base images and checked: the same configuration rules on its contents, Trivy's secret scanner, and Trivy's vulnerability scan. A weekly run repeats the image build and scans on a fresh CVE database. Nothing is published or deployed from CI; deployments build from the repository.
- GitHub secret scanning and push protection are enabled on the repository.
- Accepted findings are listed with a reason and an expiry: `.trivyignore`, `NuGetAuditSuppress` in `src/Directory.Build.props`, and `.gitleaksignore` for findings in old commits.
- Dependabot proposes weekly NuGet, Docker and GitHub Actions updates.
- Responses are never cacheable: every service and the gateway send `Cache-Control: no-store` and strip any caching headers (including CDN-specific ones); CI fails if server-side response caching is added. Tests cover a service that tries to make its responses cacheable.
- `src/auth-negative-test.py` checks a running stack: every route of every service must refuse missing, malformed, unsigned and forged tokens.
