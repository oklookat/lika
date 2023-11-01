# lika

Symlink installer. You can do symlinks in Windows without boilerplate cmd code.

The program was created with a focus on creating symlinks for VST plugins. But nevertheless it can be used for other things as well.

## Short guide

1. Download program from releases
2. Create installer folder structure (optional)
3. Create and fill ```installer.json``` file
4. Place lika.exe to dir where installer.json placed
4. Use lika.exe to install/uninstall symlinks.

installer.json:
```jsonc
{
    "installerName": "", // optional
    "author": "", // optional
    "authorUrl": "", // optional

    "links": [
        {
            // path to source file/dir, optional (!)
            /* 
            (!) You can choose not to specify src, and only specify target if your installer structure is similar to the Windows directory structure.

            Example target: "<CommonProgramFiles>/VST3/lol.vst3"
            If no src is specified, then the path of your dirs should be like this (relative to the folder where lika.exe is located):
            "C/Program Files/Common Files/VST3/lol.vst3"
            I.e:
            - lika.exe
            - C
            -  Program Files
            -    Common Files
            -      etc

            Another example target: "<PublicDocuments>/lolDir"
            Expected: "C/Users/Public/Documents/lolDir"

            see https://github.com/oklookat/lika/blob/8fdaf8d2d22749e9d9fca1a68b380b9f8bc59d10/Utils.cs#L156
            */
            "src": "",
            // required, path to target, directories will be created if they do not exist
            // deleting links (while uninstall) will delete the specific link(s). Empty directories before it will not be deleted
            "target": "",
            // if true and src is dir, symlinks will be created not to the directory itself, but to all content in it (not recursively)
            "dirContents": false, // optional, default false
            // if dirContents enabled, skip dirs/files names
            "dirContentsExcept": [] // optional
        }
    ],

    // registry, optional
    "reg": {
        // use env vars in reg file?
        /* Vars example:
        [HKEY_LOCAL_MACHINE\software\WOW6432Node\Test]
        "KeyWithVars"="<CommonProgramFiles86>\\Dir"
        */
        // use variables with caution, in case the registry value contains non-standard/bad paths, 
        // there may be unexpected results because of variables (I wrote a simple parser, so yeah)
        "installWithVars": false, // optional, default false

        // path to registry file that will be executed after installing
        // tip: if you using vars, rename .reg file to .regx or something like this
        // so you don't forget it's an unusual .reg file.
        "install": "", // optional

        "uninstallWithVars": false, // optional, default false
        "uninstall": "" // optional
    },

    // run process, optional
    "process": {
        // path to file that will be executed after installing (example: .cmd or .exe file)
        "install": "", // optional
        "uninstall": "" // optional
    }
}
```

installer.json example:
```json
{
	"installerName": "Massive v1.5.11",
	"links": [
		{
			"target": "<CommonProgramFiles>/Native Instruments/Service Center/Massive.xml"
		},
		{
			"target": "<CommonProgramFiles>/Native Instruments/Service Center/NativeAccess.xml"
		},
		{
			"target": "<CommonProgramFiles>/VST3/Massive.vst3"
		},
		{
			"target": "<ProgramFiles>/Native Instruments/Massive"
		},
		{
			"target": "<CommonProgramFiles86>/Native Instruments/Kore 2/Database Cache/Massive.kdb"
		},
		{
			"target": "<CommonProgramFiles86>/Native Instruments/Massive"
		},
		{
			"target": "<CommonProgramFiles86>/Native Instruments/Shared Content/Sounds/Massive"
		},
		{
			"target": "<PublicDocuments>/NI Resources/image/Massive"
		}
	],
	"reg": {
		"installWithVars": true,
		"install": "<CWD>/REG/add.regx",
		"uninstall": "<CWD>/REG/remove.reg"
	}
}
```

## Variables

- Current dir (relative to lika.exe): \<CWD>
- System dir: \<SystemDir>
- Current user documents: \<UserDocuments>
- Public documents: \<PublicDocuments>
- Program files: \<ProgramFiles>
- Program files (x86): \<ProgramFiles86>
- Common program files: \<CommonProgramFiles>
- CommonProgramFiles86: \<CommonProgramFiles86>
- Current user dir: \<CurrentUser>

# Requirements

- Windows 7 and above
- [NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Good mood](https://www.youtube.com/watch?v=IFzkSITcPII)
