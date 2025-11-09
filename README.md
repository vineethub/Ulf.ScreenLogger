# Ulf.ScreenLogger

Ulf.ScreenLogger is a prototype screen logging tool in C# that captures screen tiles and encodes them for efficient storage. It uses GDI+ APIs on Windows and supports compression with LZ4 and hashing with xxHash64.

---

## ⚙️ Prerequisites

- **Windows 10+** (System.Drawing APIs require Windows)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Optional: Visual Studio or VS Code

---

## 📦 Setup

1. Clone the repository:

```bash
git clone https://github.com/<your-username>/Ulf.ScreenLogger.git
cd Ulf.ScreenLogger/Ulf.ScreenLogger
```

2. Restore NuGet packages:

```bash 
dotnet restore
```

3. Build the project:

```bash
dotnet build
```

#### ⚠️ Make sure the K4os.Hash.xxHash version is compatible (>=1.0.8) to avoid build errors.