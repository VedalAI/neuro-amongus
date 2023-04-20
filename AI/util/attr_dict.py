class AttrDict(dict):
    def __getattr__(self, key):
        try:
            return self[key]
        except KeyError:
            raise AttributeError(f"'{self.__class__.__name__}' object has no attribute '{key}'")

    def __setattr__(self, key, value):
        self[key] = value

    def __getitem__(self, key):
        value = super().__getitem__(key)
        if isinstance(value, dict) and not isinstance(value, AttrDict):
            value = AttrDict(value)
            self[key] = value
        if isinstance(value, list) and any(isinstance(x, dict) for x in value):
            value = [AttrDict(x) if isinstance(x, dict) else x for x in value]
            self[key] = value
        return value
