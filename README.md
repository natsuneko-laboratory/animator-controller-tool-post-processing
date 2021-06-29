# AnimatorControllerToolPostProcessing

Rewrite the behavior of AnimatorControllerTool to be the default setting recommended by VRChat.

- When Animator Window is opened, set the Weight to 1.0f as default value for newly added layer (normal behaviour is set Weight to 0.0f)
- When Animator Window is opened, set the Write Default to False as default value for newly added state (normal behaviour is set Write Default to True)

## Requirements

- Unity 2018.4.20f1

## Installation

1. Download UnityPackage from BOOTH (Recommended)
2. Install via NPM Scoped Registry

### Download UnityPackage

You can download latest version of UnityPackage from BOOTH (Not Yet Provided).
Extract downloaded zip package and install UnityPackage into your project.

### Install via NPM

Please add the following section to the top of the package manifest file (`Packages/manifest.json`).
If the package manifest file already has a `scopedRegistries` section, it will be added there.

```json
{
  "scopedRegistries": [
    {
      "name": "Mochizuki",
      "url": "https://registry.npmjs.com",
      "scopes": ["moe.mochizuki"]
    }
  ]
}
```

And the following line to the `dependencies` section:

```json
"moe.mochizuki.animator-controller-tool-post-processing": "VERSION"
```

## How to use

None, this editor extension is automatically enabled/worked

## License

MIT by [@6jz](https://twitter.com/6jz)
