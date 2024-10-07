```
Assets/
│
├── Core/                 # Zentrale Elemente des Spiels
│   ├── GameStates/       # Zustandsmaschinen, wie Spielerzug, KI-Zug, etc.
│   ├── Events/           # Event-System für lose Kopplung zwischen Modulen
│   ├── Utilities/        # Hilfsfunktionen, Tools und generische Klassen
│   └── Managers/         # Zentrale Manager (z.B. InputManager, SceneManager)
│
├── Data/                 # Datenhaltung und ScriptableObjects
│   ├── Units/            # Scriptable Objects für Einheiten-Daten (z.B. Typen, Stats)
│   ├── Buildings/        # Scriptable Objects für Gebäude-Daten
│   ├── Technologies/     # Daten zu Technologien und Fortschritten
│   ├── Resources/        # Daten zu Spielressourcen (z.B. Gold, Nahrung)
│   └── Factions/         # Fraktionsdaten (z.B. Eigenschaften, Boni)
│
├── Models/               # Reine Datenklassen (Spielzustand, Einheiten, Städte)
│   ├── Map/              # Model-Klassen für die Karte (Tiles, Ressourcen)
│   ├── Units/            # Model-Klassen für Einheiten (Position, Gesundheit)
│   ├── Cities/           # Model-Klassen für Städte (Bevölkerung, Produktion)
│   └── Diplomacy/        # Model-Klassen für diplomatische Beziehungen
│
├── Views/                # Präsentationsebene, UI, 3D-Modelle
│   ├── UI/               # User Interface Elemente (z.B. HUD, Menüs)
│   ├── Units/            # 3D Modelle und Animationen der Einheiten
│   ├── Cities/           # Darstellung der Städte (z.B. Stadtsymbole, 3D-Modelle)
│   └── Map/              # Darstellung der Karte (z.B. Gelände, Ressourcen)
│
├── Controllers/          # Steuerung der Interaktionen zwischen Model und View
│   ├── Map/              # Controller für die Kartensysteme (z.B. MapManager)
│   ├── Units/            # Controller für Einheiten-Logik (z.B. Bewegung)
│   ├── Cities/           # Controller für Stadtaktionen (Bauen, Bevölkerung)
│   ├── Diplomacy/        # Controller für Diplomatie-Mechaniken
│   └── AI/               # Controller für die KI-Logik (Entscheidungen der KI)
│
├── AI/                   # Separate KI-Logik und Entscheidungsalgorithmen
│   ├── BehaviorTrees/    # Verhaltensbäume für die KI
│   ├── AIStates/         # Zustände und Aktionen der KI (z.B. Angreifen, Handeln)
│   └── AIControllers/    # Steuerung der KI-Entscheidungen und Aktionen
│
├── Prefabs/              # Vorlagen für Einheiten, Gebäude, Karten-Tiles
│   ├── Units/            # Einheiten-Prefabs
│   ├── Buildings/        # Gebäude-Prefabs
│   └── Map/              # Prefabs für Kartenelemente (z.B. Tiles)
│
├── Scenes/               # Szenen für das Spiel (z.B. Hauptmenü, Spielwelt)
│   ├── MainMenu/         # Hauptmenü und UI-Szenen
│   └── GameWorld/        # Spielwelt-Szenen (mit Karte und Spielzustand)
│
├── Resources/            # Allgemeine Ressourcen wie Materialien, Texturen, Sounds
│   ├── Textures/         # Texturen für die Karte, Einheiten, etc.
│   ├── Materials/        # Materialien für 3D-Modelle
│   └── Audio/            # Sound-Effekte und Musik
│
├── Animations/           # Animationen für Einheiten, Gebäude und UI-Elemente
│   ├── Units/            # Animationen für Einheitenbewegungen und Kämpfe
│   └── UI/               # Animationen für UI-Elemente (z.B. Übergänge)
│
└── Tests/                # Unit-Tests, Integrationstests
    ├── UnitTests/        # Tests für einzelne Systemkomponenten (z.B. Model-Tests)
    └── IntegrationTests/ # Tests für die Interaktion zwischen Systemen (z.B. Controller und View)
```
