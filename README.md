# AI COMPANION

A lightweight, always-on-top floating assistant that accepts natural language commands and performs contextual actions like playing YouTube videos, opening apps/websites, or reading PDFs. Built using **WPF**, **C#**, and integrated with **Gemini/OpenRouter AI** models for smart interaction.

<br/>

## Features

- `@notepad-title content` → Save quick notes locally.
- `@youtube-video_name` → Auto-play top matching YouTube video.
- `@website-domain.com` → Open any website in default browser.
- `@app-app_name` → Launch installed apps (e.g. Notepad, Calc).
- `@pdf-filename` → Open local PDF files from `Documents`.

<br/>

## Tech Stack

| Tech        | Purpose                          |
|-------------|----------------------------------|
| WPF (.NET)  | UI and floating overlay system   |
| C#          | App logic, command parsing       |
| Regex       | Natural command extraction       |
| HttpClient  | YouTube scraping and AI API calls|
| HtmlAgilityPack | Parse YouTube search results |
| OpenRouter/Gemini | AI-based chat/response     |

<br/>

## Getting Started

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/en-us/download)
- Visual Studio (recommended)
- NuGet packages:
  - `HtmlAgilityPack`
  - `Newtonsoft.Json`

```bash
Install-Package HtmlAgilityPack
Install-Package Newtonsoft.Json

