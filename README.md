# DAIKON: Data Acquisition, Integration, and Knowledge Capture Web Application for Target-Based Drug Discovery

## Overview

DAIKON is an open-source framework designed to streamline target-based drug discovery processes. It integrates targets, screens, hits, and manages projects within a drug discovery portfolio. The application captures and visualizes the progress of each target, facilitating effective collaboration among scientific teams.

## Key Features

- **Modular Design**: DAIKON is structured to allow easy extensions and adaptations. It abstracts the database and creates separate layers for entities, business logic, infrastructure, APIs, and frontend.
- **Knowledge Capture**: Teams can record and analyze data, track molecular properties, and facilitate discussions through integrated threads.
- **Data Integration**: Sources data from Mycobrowser, UniProt, and PDB, supporting a unified approach to data acquisition and analysis.
- **Project Management**: Manages multiple projects across various phases of drug discovery, providing a holistic view of targets and compounds.

## Technical Architecture

DAIKON is developed using .NET Core and React JS, employing a client-server architecture that communicates via JSON APIs. It now uses a microservice architecture, where each service is independently deployable and scalable. This architecture improves maintainability and flexibility, allowing for targeted updates and better resource management. The framework can be deployed on on-premises servers or private clouds, supporting Active Directory/SSO for user administration.

## Contribution
Contributions are welcome! Please fork the repository and create a pull request with your changes. Ensure that your code follows the project's coding standards and includes appropriate tests.

## License
DAIKON is released under the MIT License. See LICENSE for more details.

## Acknowledgements
This project is supported by the Bill & Melinda Gates Foundation and the NIH. We are thankful for the contributions and support from various research organizations and individuals.

## Links
Publication (https://pubs.acs.org/doi/10.1021/acsptsci.3c00034)
Rath S, Panda S, Sacchettini JC, Berthel SJ. DAIKON: A Data Acquisition, Integration, and Knowledge Capture Web Application for Target-Based Drug Discovery. ACS Pharmacol Transl Sci. 2023 Jun 22;6(7):1043-1051. doi: 10.1021/acsptsci.3c00034. PMID: 37470023; PMCID: PMC10353056.
