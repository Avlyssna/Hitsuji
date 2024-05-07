# Hitsuji

[![Downloads](https://img.shields.io/github/downloads/Avlyssna/Hitsuji/total.svg)](https://github.com/Avlyssna/Hitsuji/releases)
[![License](https://img.shields.io/github/license/Avlyssna/Hitsuji.svg)](LICENSE)

**Free, Open-Source Remote Administration Tool for Windows**

Hitsuji is a fork of the popular [Quasar](https://github.com/quasar/Quasar) tool.
It is designed for use as a red-team persistence tool during penetration testing.

Please check out the [Getting Started](https://github.com/quasar/Quasar/wiki/Getting-Started) guide.

## Screenshots

![remote-shell](Images/remote-shell.png)

![remote-desktop](Images/remote-desktop.png)

![remote-files](Images/remote-files.png)

## Features
* TCP network stream (IPv4 & IPv6 support)
* Fast network serialization (Protocol Buffers)
* Encrypted communication (TLS)
* UPnP Support (automatic port forwarding)
* Task Manager
* File Manager
* Startup Manager
* Remote Desktop
* Remote Shell
* Remote Execution
* System Information
* Registry Editor
* System Power Commands (Restart, Shutdown, Standby)
* Keylogger (Unicode Support)
* Reverse Proxy (SOCKS5)
* Password Recovery (Common Browsers and FTP Clients)
* ... and many more!

## Download
* [Latest stable release](https://github.com/Avlyssna/Hitsuji/releases) (recommended)

## Building a client
| Build configuration         | Usage scenario | Description
| ----------------------------|----------------|--------------
| Debug configuration         | Testing        | The pre-defined [Settings.cs](/Hitsuji.Client/Config/Settings.cs) will be used, so edit this file before compiling the client. You can execute the client directly with the specified settings.
| Release configuration       | Production     | Start `Hitsuji.exe` and use the client builder.

## Contributing
See [CONTRIBUTING.md](CONTRIBUTING.md)

## Documentation
See the [wiki](https://github.com/Avlyssna/Hitsuji/wiki) for usage instructions and other documentation.

## License
Hitsuji is distributed under the [MIT License](LICENSE).
Third-party licenses are located [here](Licenses).
