find . \( -name '*.cs' -o -name '*.py' -o -name '*.yml' \) -print0 | xargs -0 cat | wc -l
