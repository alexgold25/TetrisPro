# TetrisPro AI Module

This document outlines the basic usage and tuning options for the experimental `AiAgent`.

## Usage

```csharp
var agent = new AiAgent();
AiDecision decision = agent.DecideNextMove(gameState);
foreach (var e in decision.InputPlan)
    inputService.KeyDown(e.Key);
```

## Configuration

The agent exposes several parameters via constructors:

- `EvalWeights` – heuristic weights controlling the evaluator.
- `ply` / `beamWidth` – search depth and width.
- `HumanInputEmulator` – timings for DAS/ARR/Tap and jitter.

Adjusting these allows the AI to be tuned for different play styles.
