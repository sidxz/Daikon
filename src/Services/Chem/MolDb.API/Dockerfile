# Start with a Miniconda3 base image
FROM continuumio/miniconda3

WORKDIR /app


# Set the user to root to install packages
USER root

# Update and install system dependencies if needed
RUN apt-get update && apt-get install -y \
    cmake \
    flex \
    bison \
    libboost-all-dev

# Create a Conda environment and install necessary packages
RUN conda create -n myenv python=3.8
RUN echo "source activate myenv" > ~/.bashrc
ENV PATH /opt/conda/envs/myenv/bin:$PATH

# Install RDKit and other Python packages using Conda
RUN conda install -c conda-forge -n myenv rdkit
RUN conda run -n myenv pip install fastapi[all] uvicorn psycopg2-binary asyncpg marshmallow confluent_kafka

COPY ["Services/Chem/MolDb.API/", "/app/"]

# CMD should use the binaries from the Conda environment
CMD ["uvicorn", "app.main:app", "--host", "localhost", "--port", "8101"]
