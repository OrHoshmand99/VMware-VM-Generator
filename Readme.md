# VMware Virtual Machine Generator

A lightweight, advanced, and user-friendly Windows Forms wizard (C# / .NET) designed to simplify and accelerate the process of creating, configuring, and deploying virtual machines in VMware Workstation.

## ✨ Key Features

* **Multi-Step Wizard Interface:** Guides the user step-by-step—from specifying the virtual machine name and location to configuring advanced hardware and generating shortcuts.
* **Automatic Installation Check:** Automatically verifies if VMware Workstation is installed on the system and detects installation paths (including validation of `vmware-vdiskmanager.exe`).
* **Comprehensive Hardware Management:**
  * **Processors:** Select the number of processors and cores per processor with an automatic calculation of total cores.
  * **Memory (RAM):** Interactive memory trackbar with recommended minimum, default, and maximum value indicators.
  * **Firmware:** Support for BIOS or UEFI firmware configuration.
* **Controllers and Peripherals:**
  * Sound card configuration (Sound Blaster 16, Ensoniq AudioPCI, Intel HD Audio).
  * SCSI I/O controllers (BusLogic, LSI Logic, LSI Logic SAS, Paravirtualized SCSI).
  * Network connection types (Bridged, NAT, Host-Only, or no network).
* **Virtual Hard Disks & Storage:**
  * Support for multiple virtual disk types (IDE, SCSI, SATA, NVMe).
  * Configure capacity, disk preallocation, and splitting disk files for easier portability.
  * Manage floppy drives and multiple optical (CD/DVD) drives across various controllers (IDE, SCSI, SATA).
* **Shortcuts & Advanced Options:**
  * Create desktop and start menu shortcuts.
  * Support for direct VMX file shortcuts, KVM shortcuts, or custom Workstation shortcuts (including opening in a new window, auto-start, and full-screen modes).
  * Custom virtual machine icon selection and extraction using native Windows pickers and DLL extraction.
* **Localization & Flexibility:** Full support for external configuration and language INI files (`settings.ini`), including RTL layout support.

## 🛠️ Tech Stack

* **Language:** C# (.NET 10.0 / Windows Forms)
* **Libraries & Tools:** P/Invoke for native Windows system dialogs and icon extraction (`shell32.dll`), `vdiskmanager` for real virtual disk provisioning, and Costura.Fody for single-file executable packaging.

## 🚀 Getting Started

1. Ensure **VMware Workstation** is installed on your computer.
2. Clone or download this repository.
3. Build and run the project using Visual Studio or the .NET CLI (`dotnet run`).