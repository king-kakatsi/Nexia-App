# Nexia

**Nexia** is a cross-platform mobile application built with Xamarin.Forms, designed for real-time face detection and recognition. Leveraging advanced AI via the Face++ API, Nexia delivers a seamless, interactive, and futuristic user experience for both Android and iOS.

---

## 🚀 Features

- **Face Detection**: Instantly scan and detect faces from photos or camera input.
- **Attribute Recognition**: Accurately identifies age and gender.
- **Interactive UI**: Animated scanning interface with sound and voice feedback.
- **Usage Limiting**: Built-in quota system to restrict usage (demo/limited mode).
- **Modern Design**: High-tech visuals, custom fonts, and smooth transitions.
- **Cross-Platform**: Runs on both Android and iOS with a unified codebase.

---

## 🛠️ Requirements

- **Xamarin.Forms** 5.0+
- **.NET Standard 2.0**
- **Android**: API 21+  
- **iOS**: iOS 10.0+
- **Face++ API Key & Secret** (default demo keys included, but you should use your own for production)
- **Internet Connection** (required for face detection)

---

## 📦 Dependencies

- [Xamarin.Forms](https://www.nuget.org/packages/Xamarin.Forms)
- [Xamarin.Essentials](https://www.nuget.org/packages/Xamarin.Essentials)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)
- [Xam.Plugin.Media](https://www.nuget.org/packages/Xam.Plugin.Media)
- [Xam.Plugin.SimpleAudioPlayer](https://www.nuget.org/packages/Xam.Plugin.SimpleAudioPlayer)
- [Xamarin.CommunityToolkit](https://www.nuget.org/packages/Xamarin.CommunityToolkit)
- [Com.Airbnb.Xamarin.Forms.Lottie](https://www.nuget.org/packages/Com.Airbnb.Xamarin.Forms.Lottie)

---

## ⚙️ Setup & Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/nexia.git
   ```

2. **Open the solution**  
   Open `Nexia.sln` in Visual Studio 2019 or later.

3. **Restore NuGet packages**  
   Visual Studio will prompt you, or right-click the solution and select `Restore NuGet Packages`.

4. **Configure API Keys**  
   - The app uses demo Face++ API keys by default.
   - For production, get your own keys from [Face++](https://www.faceplusplus.com/) and update them in `Nexia/services/CognitiveService.cs`:
     ```csharp
     private static string API_KEY = "YOUR_API_KEY";
     private static string API_SECRET = "YOUR_API_SECRET";
     ```

5. **Build and Run**  
   - Select the target platform (Android/iOS).
   - Deploy to an emulator or physical device.

---

## 🧑‍💻 Usage

1. **Launch the app.**
2. **Tap "Scan a face".**
3. **Choose to take a new photo or select one from your gallery.**
4. **Watch the animated scan and hear sound/voice feedback.**
5. **View detected attributes (age, gender) on the result screen.**

> **Note:** The app enforces a usage limit (demo mode). If you reach the limit, a message will prompt you to contact the developer.

---

## 🧬 Data Model

The face detection response includes:
```json
{
  "faces": [
    {
      "face_token": "...",
      "face_rectangle": { "top": 0, "left": 0, "width": 0, "height": 0 },
      "attributes": {
        "gender": { "value": "Male" },
        "age": { "value": 28 }
      }
    }
  ],
  "face_num": 1,
  "image_id": "...",
  "request_id": "...",
  "time_used": 0
}
```

---

## 💡 Suggestions

- Replace demo API keys with your own for higher limits and privacy.
- Customize the UI, sounds, and feedback for your brand or use case.
- Integrate additional Face++ features (emotion, ethnicity, etc.) for richer results.
- Remove or adjust the usage limit for production deployments.

---

## 🛡️ Security & Privacy

- All face detection is performed via the Face++ API.
- No images or personal data are stored locally or transmitted elsewhere.
- For production, secure your API keys and consider implementing authentication.

---

## 📝 License

MIT License.  
Developed by King Kakatsi.

---

## 🤝 Contact

For support, feature requests, or to unlock the app, visit [kingweb.pythonanywhere.com](https://kingweb.pythonanywhere.com) or contact the developer.

**WhatsApp:** [+233535610908](https://wa.me/233535610908)

---

## 🌟 Acknowledgements

- [Face++](https://www.faceplusplus.com/) for their powerful face detection API.
- Xamarin.Forms and the open-source community.

---

## 🔑 API Keys & Secrets

Sensitive API keys (such as Face++ credentials) are stored in `Nexia/services/SecretKeys.cs`, which is excluded from version control via `.gitignore` for security.

- **Onboarding:**
  - Copy the following template to create your own `SecretKeys.cs` in `Nexia/services/`:
    ```csharp
    namespace Nexia.services
    {
        public static class SecretKeys
        {
            public static string API_KEY { get; } = "YOUR_API_KEY";
            public static string API_SECRET { get; } = "YOUR_API_SECRET";
        }
    }
    ```
  - Replace `YOUR_API_KEY` and `YOUR_API_SECRET` with your actual Face++ credentials.

- **Note:** Never commit your real `SecretKeys.cs` to version control. The file is already listed in `.gitignore`. 