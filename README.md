# lika

**attention: this program tested on 2 plugins only**

If you use VST plugins, you probably know about their symlink versions. It's a very handy thing, isn't it? 
And if you have created such versions, here's a question: aren't you tired of writing cmd or bat files? The question is rhetorical.

So I tried to replace bat symlinks with this program.

The .exe reads information from "installer.json", which tells what folders to create and what links to create. 
This file is also used to delete symlinks. In short: replace bat's with lika.exe, write installer.json.

## Example installer.json (Massive X):

**tip: remove comments if you will use this example**

```javascript
{
  "installerName": "Massive X", // optional
  "author": "TotallyLegitimateSoftwareUser", // optional
  "authorUrl": "https://example.com", // optional
  "create": [
    "<ProgramFiles>/Native Instruments",
    "<CommonProgramFiles>/Native Instruments",
    "<ProgramFiles>/VstPlugins/Native Instruments",
    "<CommonProgramFiles>/Common Files/VST3",
    "<UserDocuments>/Native Instruments/User Content",
    "<PublicDocuments>/NI Resources/image"
  ], // optional
  "links": [
    {
      "src": "<CWD>/C/Program Files/Common Files/Native Instruments/Massive X", // symlink source
      "target": "<CommonProgramFiles>/Native Instruments/Massive X" // symlink target
    },
    {
      "src": "<CWD>/C/Program Files/Common Files/VST3/Massive X.vst3",
      "target": "<CommonProgramFiles>/VST3/Massive X.vst3"
    },
    {
      "src": "<CWD>/C/Program Files/VstPlugins/Native Instruments/Massive X.dll",
      "target": "<ProgramFiles>/VstPlugins/Native Instruments/Massive X.dll"
    },
    {
      "src": "<CWD>/C/Users/(current user)/Documents/Native Instruments/User Content/Massive X",
      "target": "<UserDocuments>/Native Instruments/User Content/Massive X"
    },
    {
      "src": "<CWD>/C/Users/Public/Documents",
      "target": "<PublicDocuments>",
      "dirContents": true, // optional, symlink all content from source
      "dirContentsExcept": ["NI Resources"] // optional, except this folder names
    },
    {
      "src": "<CWD>/C/Users/Public/Documents/NI Resources/image",
      "target": "<PublicDocuments>/NI Resources/image",
      "dirContents": true
    }
  ],
  "reg": {
    "install": "<CWD>/reg/add.reg", // optional, exec reg file after install
    "uninstall": "<CWD>/reg/remove.reg" // optional, exec reg file after uninstall
  } // optional
}
```

## Variables

- lika.exe dir: \<CWD>
- System dir: \<SystemDir>
- Current user documents: \<UserDocuments>
- Public documents: \<PublicDocuments>
- Program files: \<ProgramFiles>
- Program files (x86): \<ProgramFiles86>
- Common program files: \<CommonProgramFiles>
- CommonProgramFiles86: \<CommonProgramFiles86>
- Current user dir: \<CurrentUser>

# Requirements

- [NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Windows 7+
- [Good mood](https://www.youtube.com/watch?v=IFzkSITcPII)

# TODO

The program will (probably) be updated as needed.

For example, .reg files often use paths like "C:/Users/etc", 
which means that although we use environment variables, .reg files don't use them. 
In short, I would like to add something like a .reg file editor to the program to change the paths to actual ones. 
But I'm too lazy to do that yet, so yeah.
