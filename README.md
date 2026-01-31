# Projet VR Industriel - Incarnation & IA Compagnon (TP3/4/5)

**Auteur :** [TON NOM]
**Date :** Janvier 2026
**Moteur :** Unity 2022.3 (URP)

---

## Vue d'ensemble
Ce projet combine un environnement industriel immersif (TP3), une incarnation complète du joueur avec corps physique (TP4), et une IA de drone compagnon capable de naviguer sans NavMesh (TP5).

---

## TP3 : Rendu & Shaders (URP)
* [cite_start]**Environnement :** Scène industrielle optimisée avec collisions gérées sur les layers `Default` et `Industrial`[cite: 22, 49].
* [cite_start]**Arme & Feedback :** Implémentation d'un **MaterialPropertyBlock** pour modifier les propriétés `_Hover` et `_Grab` du shader sans instancier de nouveaux matériaux, garantissant des performances optimales[cite: 17, 22].

---

## TP4 : Incarnation & Interaction
* [cite_start]**Système IK :** Utilisation de contraintes *Two Bone IK* pour les bras et *Multi-Aim* pour le torse[cite: 24].
* **Auto-Pickup :** Système de ramassage automatique basé sur la physique (Trigger) et l'animation. L'arme se parente au `WeaponSocket` lors d'un événement précis de l'animation *Equip*.
* [cite_start]**Stabilisation :** Application d'un lissage (Damping) sur la position et la rotation des mains pour éviter le *jitter* des contrôleurs VR[cite: 19, 24].

---

## TP5 : Pathfinding Avancé (Drone Compagnon)

[cite_start]Le drone utilise l'algorithme **RB-3DCP** (Ray-Based 3D Constrained Pursuit) pour suivre le joueur en évitant les obstacles dynamiquement via `SphereCast`[cite: 66, 72].

### [cite_start]A. Paramètres de l'Algorithme (Livrable A) [cite: 61]

Voici les valeurs retenues pour garantir une navigation fluide dans des couloirs étroits :

| Paramètre | Valeur | Justification |
| :--- | :--- | :--- |
| **LookAhead Distance** | `3.0 m` | [cite_start]Permet d'anticiper les murs suffisamment tôt à la vitesse de 3m/s pour virer sans collision brute[cite: 57]. |
| **Clearance Radius** | `0.4 m` | Rayon du drone (0.3m) + Marge de sécurité (0.1m). [cite_start]Empêche le mesh de "gratter" les murs lors des virages serrés[cite: 58]. |
| **Stratégie de Scan** | `Scan Déterministe` | Au lieu de rayons aléatoires, le drone teste des angles précis (`0`, `±15`, `±30`, etc.). [cite_start]Cela évite le tremblement (jitter) typique des méthodes stochastiques[cite: 90]. |
| **Cone Angle** | `Max 80°` | [cite_start]Un cône large est nécessaire pour trouver une issue lorsque le drone est face à un mur plat ou dans un coin (Cul-de-sac)[cite: 56]. |
| **Damping (Rotation)** | `10.0` | [cite_start]Une réactivité élevée est nécessaire pour que le drone s'oriente immédiatement vers la sortie du couloir dès qu'elle est détectée[cite: 94]. |

### B. Comportements & Feedback
* **Navigation :** Le drone priorise toujours la ligne directe vers l'*Anchor* (épaule du joueur). [cite_start]Si bloqué, il scanne séquentiellement les angles latéraux[cite: 68].
* [cite_start]**Feedback Visuel :** Utilisation de `MaterialPropertyBlock` pour changer la couleur du drone (Bleu = Suivi, Rouge = Obstacle/Recherche)[cite: 32].
* [cite_start]**Animation Procédurale :** Le drone s'incline ("Tilt") dynamiquement selon sa vitesse et sa rotation pour donner une impression de physique aérienne réaliste[cite: 34].

---

### [cite_start]Instructions pour tester la scène [cite: 30]
1.  Lancer la scène `MainScene`.
2.  Avancer : Le corps suit le mouvement (TP4).
3.  Marcher sur l'arme au sol : L'animation se lance et l'arme s'équipe (TP3/4).
4.  Entrer dans le "Couloir de Test" (Cubes gris) : Le drone se faufile derrière le joueur sans toucher les murs (TP5).
