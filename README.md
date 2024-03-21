# DialogosProject

Welcome to the DialogosProject repository! This repository contains the Unity project for the Dialogos system, an advanced platform for training AI agents using machine learning techniques. The project leverages Unity's ML-Agents toolkit to create a robust environment for non-deterministic and fully stochastic agent behavior.

## Getting Started

To get started with the DialogosProject, clone this repository to your local machine using your preferred Git client.

```bash
git clone https://github.com/yourusername/DialogosProject.git
```

Open the project in Unity Hub by navigating to the cloned repository folder.

## Bootstrap Process

The project includes a bootstrap system that initializes core systems, loads entity data, starts asynchronous jobs, and executes static function calls. This process ensures that all systems and entities are correctly initialized and ready for operation.

### Bootstrap File

The `bootstrap.unityboot` file contains a list of commands executed sequentially during the bootstrap process. If the file does not exist, it is created with default content at runtime.

### Boot Sequence

After a successful boot sequence, the system transitions to a "MainMenu" scene, which serves as the entry point for users to navigate the application.

## Training AI Agents

The Dialogos system is designed to train multiple AI agents concurrently, each with a different starting seed to ensure varied learning experiences. The number of concurrent agents and other settings can be configured in the `engine.ini` file.

## Project Structure
The following is a breif high level description of the projects directory structure:

```
- `/Assets/`: Contains all the assets, scripts, and resources used in the Unity project.
- `|-Scripts/`: Includes C# scripts for the bootstrap manager and other components.
- `|-Scenes/`: Holds the Unity scenes, including the "MainMenu" scene.
- `|-Config/`: Stores configuration files like `engine.ini`.
```
## Contributing

We welcome contributions to the DialogosProject! Please read our contributing guidelines before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Unity Technologies for the ML-Agents toolkit
- All contributors and supporters of the DialogosProject

Thank you for visiting the DialogosProject repository. We look forward to your contributions and feedback!
