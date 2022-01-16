# AnimatorControllerToolPostProcessing

Rewrite the behavior of AnimatorControllerTool to be the default setting recommended by VRChat.

- When Animator Window is opened, set the Weight to 1.0f as default value for newly added layer (normal behaviour is set Weight to 0.0f)
- When Animator Window is opened, set the Write Default to False as default value for newly added state (normal behaviour is set Write Default to True)

## Requirements

- Unity 2018.4.20f1

## Installation

1. Download UnityPackage from GitHub Releases
2. Install via NPM Scoped Registry
3. Clone GitHub Repository and Copy Scripts

### Download UnityPackage

You can download latest version of UnityPackage from GitHub Releases (Not Yet Provided).
Extract downloaded zip package and install UnityPackage into your project.

### Install via NPM

Please add the following section to the top of the package manifest file (`Packages/manifest.json`).
If the package manifest file already has a `scopedRegistries` section, it will be added there.

```json
{
  "scopedRegistries": [
    {
      "name": "Natsuneko Laboratory",
      "url": "https://registry.npmjs.com",
      "scopes": ["moe.natsuneko"]
    }
  ]
}
```

And the following line to the `dependencies` section:

```json
"moe.natsuneko.animator-controller-tool-post-processing": "VERSION"
```

## How to use

None, this editor extension is automatically enabled/worked

## License

MS-RSL by [@6jz](https://twitter.com/6jz)

If you want to see/use/modify the source code under a free license (MIT), consider supporting it with GitHub Sponsors, Patreon or Pixiv Fanbox (monthly or yearly donations are supported).
