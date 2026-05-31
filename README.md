<div align="center">
  <img src="SteamCardFarmer/Resources/icon.ico" width="80"/>
  <h1>Unlock Steam Cards</h1>
  <p>Farm tes cartes à échanger Steam automatiquement — sans jouer.</p>

  ![Platform](https://img.shields.io/badge/platform-Windows-blue)
  ![License](https://img.shields.io/badge/license-GPL--2.0-green)
  ![.NET](https://img.shields.io/badge/.NET-8.0-purple)
</div>

---

## 📥 Installation (1 minute)

> ### ⭐ Méthode recommandée — Installeur (le plus simple)
> 1. Va dans **[Releases](../../releases/latest)**
> 2. Télécharge **`UnlockSteamCards-Setup.exe`**
> 3. Lance l'installeur et suis les étapes
> 4. Un raccourci apparaît sur ton Bureau — clique dessus et c'est parti !
>
> ✅ Pas besoin d'installer .NET — tout est inclus.  
> ✅ `steam_api64.dll` détecté automatiquement depuis tes jeux Steam.  
> ✅ Fonctionne sur **tous les disques** (C:, D:, E:...).

---

## 🔒 100% Sûr et Transparent

- ✅ **Code source ouvert** — tout le code est visible sur ce dépôt, rien de caché
- ✅ **Aucune donnée collectée** — l'app ne communique qu'avec les serveurs Steam officiels
- ✅ **Aucun mot de passe stocké** — seuls les cookies de session sont sauvegardés localement sur ton PC
- ✅ **Utilisé par des millions** — basé sur Idle Master Extended, référence depuis 2013
- ✅ **Pas de virus** — vérifie toi-même sur [VirusTotal](https://www.virustotal.com) si tu veux

---

## 🔑 Connexion

L'app utilise tes cookies Steam pour accéder à ton profil.

1. Ouvre **[steamcommunity.com](https://steamcommunity.com)** dans Chrome/Edge
2. Appuie sur **F12** → onglet **Application** → **Cookies** → `steamcommunity.com`
3. Copie la valeur de **`steamLoginSecure`**
4. Copie la valeur de **`sessionid`**
5. Colle-les dans l'app et clique **Se connecter**

---

## 🚀 Utilisation

| Étape | Action |
|-------|--------|
| 1 | Connecte-toi avec tes cookies Steam |
| 2 | Tes jeux avec des cartes restantes apparaissent automatiquement |
| 3 | Active **Fast Mode** pour idler plusieurs jeux en parallèle |
| 4 | Clique **Démarrer** — les cartes tombent toutes seules ! |

---

## ✨ Fonctionnalités

- 🃏 **Farming automatique** — idle tes jeux un par un ou en parallèle
- ⚡ **Fast Mode** — idle jusqu'à 30 jeux simultanément
- ⬆ **Priorité** — mets un jeu en tête de liste d'un clic
- 🔔 **Notifications** — alerte quand une carte tombe
- 📊 **Statistiques** — cartes farmées, temps de session
- 🌍 **8 langues** — FR, EN, ES, DE, IT, ZH, PT, JA
- 🔄 **Auto-actualisation** — la liste se met à jour toutes les 15 minutes automatiquement
- 🔒 **Réduit dans la barre** — continue en arrière-plan

---

## 🌍 Langues supportées

Français · English · Español · Deutsch · Italiano · 中文 · Português · 日本語

---

## ❓ FAQ

**Q : Est-ce sûr pour mon compte Steam ?**  
R : Oui. L'app utilise l'API officielle Steamworks, exactement comme Idle Master Extended utilisé par des millions de joueurs depuis des années.

**Q : Pourquoi le jeu apparaît comme "en cours" sur Steam ?**  
R : C'est normal — c'est exactement ce qui déclenche les drops de cartes.

**Q : Combien de temps ça prend ?**  
R : En général 2h maximum par jeu. Les vérifications se font toutes les 5 minutes (toutes les 60 secondes quand il reste 1 carte).

**Q : Mes cookies expirent ?**  
R : Si l'app dit "erreur de connexion", retourne dans les DevTools Steam et copie de nouveaux cookies.

**Q : Le farming continue si je ferme la fenêtre ?**  
R : Oui ! L'app se réduit dans la barre système (en bas à droite). Double-clique sur l'icône pour rouvrir.

---

## 🛠 Compiler soi-même

**Prérequis :** [.NET 8 SDK](https://dotnet.microsoft.com/download) + [Visual Studio 2022](https://visualstudio.microsoft.com/)

```
git clone https://github.com/TON-USERNAME/unlock-steam-cards
cd unlock-steam-cards
BUILD_RELEASE.bat
```

Le dossier `publish/` contient l'app prête à distribuer.

---

## 📄 Crédits

Inspiré de [Idle Master Extended](https://github.com/jonas-med-ett-s/idle_master_extended) par JonasNilson.  
Licence : GPL-2.0
