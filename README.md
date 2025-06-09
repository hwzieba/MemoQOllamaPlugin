This is an MT plugin for MemoQ 9.7 allowing users to use locally installed Ollama LLMs for machine-translation in MemoQ.

It does not work with MemoQ 11.

So far it has been tested on English to Polish translations using nous-hermes2, mistral 7B, llama2, and stablelm2. Out the four, the first one provided the best results, but the prompt might need some tweeking. 

The performance on my PC (Intel Core i7-4770K, RAM 16.0 GB, SSD, NVIDIA GeForce GTX 1650 4 GB) was rather low. The first response might take up to 30 seconds, but the subsequent ones - only a few.

Uses .NET 4.7.2 framework, and memoQ-MT-SDK-9.7.10.

Instructions:
1. Copy compiled .dll to Addins folder.
2. Open MemoQ project.
3. Go to project Settings-MT settings.
4. Select your settings and click Edit.
5. Enable the plugin by checking the Ollama plugin checkbox.
6. Configure plugin settings (Ollama URL and model name).
7. Click Save.