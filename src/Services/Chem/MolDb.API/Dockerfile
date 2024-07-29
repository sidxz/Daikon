# Start with a Miniconda3 base image
FROM python:3.8-slim

WORKDIR /app

# Set the user to root to install packages
USER root

# Update and install system dependencies if needed
RUN apt-get update && apt-get install -y \
    cmake \
    flex \
    bison \
    libboost-all-dev \
    pipenv

# Create a virtual environment and install necessary packages using Pipenv
COPY ["Services/Chem/MolDb.API/Pipfile", "/app/"]
COPY ["Services/Chem/MolDb.API/Pipfile.lock", "/app/"]

RUN pipenv install --deploy

# Install numpy explicitly
RUN pipenv run pip install numpy

COPY ["Services/Chem/MolDb.API/", "/app/"]

# CMD should use the binaries from the virtual environment created by Pipenv
CMD ["pipenv", "run", "uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8101"]
