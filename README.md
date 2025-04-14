# Kuiper Resource Server

**Kuiper Resource Server** is a high-performance, declarative resource API designed for developing system control planes.

---

## ✨ Features
- 🔧 Declarative resource definitions (via JSON, YAML, or C#)
- ⚡ Fast, extensible .NET Core 8+ backend
- 🧩 Modular plugin system (Kuiper.Extension.*)
- 📂 Hot-reloadable resource trees
- 🌐 Designed for desktop, server, and embedded contexts

---

## 📦 Installation

Coming soon via NuGet:

```bash
dotnet add package Kuiper.ResourceServer

## ⚙️ Development
#### Reading msbuild binlogs
winget install KirillOsenkov.MSBuildStructuredLogViewer

#### slngen
`dotnet tool install --global Microsoft.VisualStudio.SlnGen.Tool`
`slngen`

#### building
`dotnet build`