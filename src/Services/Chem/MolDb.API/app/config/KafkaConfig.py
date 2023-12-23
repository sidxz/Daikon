import os


def get_env_variable(var_name, default=None, required=False):
    value = os.getenv(var_name, default)
    if required and value is None:
        raise EnvironmentError(
            f"Required environment variable '{var_name}' is not set."
        )
    return value


KAFKA_CONFIG = {
    "bootstrap.servers": get_env_variable(
        "KafkaConsumerSettings_BootstrapServers", required=True
    ),
    "group.id": get_env_variable("KafkaConsumerSettings_GroupId", required=True),
    "auto.offset.reset": get_env_variable(
        "KafkaConsumerSettings_AutoOffsetReset", required=True
    ),
    "enable.auto.commit": get_env_variable(
        "KafkaConsumerSettings_EnableAutoCommit", "true"
    ).lower()
    == "true",
    "allow.auto.create.topics": get_env_variable(
        "KafkaConsumerSettings_AllowAutoCreateTopics", "true"
    ).lower()
    == "true",
}
