# Smart Agriculture System

## Overview

The Smart Agriculture System is a comprehensive project aimed at leveraging technology to enhance agricultural practices. It includes a Flutter-based mobile application (`agriculture_app`) and a backend system implemented in .NET Core (`Smart_Agriculture_System`).

### Features

- **Mobile Application**: A Flutter app for farmers to monitor and manage their crops, soil, and weather conditions.
- **Backend System**: A .NET Core API for handling data related to plants, soils, diseases, and weather forecasts and store it in MongoDB for data storage

## Project Structure

- `agriculture_app/`: Contains the Flutter mobile application.
- `Smart_Agriculture_System/`: Contains the .NET Core backend system.

## Getting Started

### Prerequisites

- Flutter SDK
- .NET Core SDK
- MongoDB

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/dev-mahmoudhamed/Smart_Agriculture_System
   ```
2. Navigate to the `agriculture_app` folder and run:
   ```bash
   flutter pub get
   flutter run
   ```
3. Navigate to the `Smart_Agriculture_System` folder and run:
   ```bash
   dotnet restore
   dotnet run
   ```

## License

This project is licensed under the MIT License.