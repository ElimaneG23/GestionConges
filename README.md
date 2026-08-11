# Gestion des Congés — Backend .NET (SaaS multi-tenant)

API REST ASP.NET Core 8 (Clean Architecture) pour une plateforme SaaS de gestion des congés,
avec 4 rôles : **SuperAdmin**, **Admin** (entreprise cliente), **Manager**, **Employé**.

## Architecture

```
GestionConges.sln
src/
  GestionConges.Domain/          Entités métier, enums (aucune dépendance externe)
  GestionConges.Application/     DTOs, interfaces, services métier (règles de gestion)
  GestionConges.Infrastructure/  EF Core, JWT, hashing des mots de passe
  GestionConges.API/             Controllers, Program.cs, configuration DI, Swagger
```

Principe : `API → Infrastructure → Application → Domain` (les dépendances pointent vers
le centre ; le Domain ne dépend de rien).

## Modèle multi-tenant

- `Tenant` = une entreprise cliente (nom, sous-domaine, logo, couleurs).
- `User.TenantId` est **null pour le SuperAdmin** (il gère la plateforme, pas une entreprise).
- Chaque requête authentifiée porte un claim JWT `tenantId` ; tous les services filtrent
  systématiquement leurs requêtes par ce `TenantId` pour garantir l'isolation des données
  entre entreprises.
- Le SuperAdmin crée les tenants (`POST /api/tenants`) ; l'Admin de chaque entreprise
  paramètre ensuite son branding (`PUT /api/tenant-settings/branding`) et ses règles RH
  (`PUT /api/tenant-settings/hr-rules`).

## Rôles et permissions (policies)

| Rôle       | Peut faire                                                            |
|------------|------------------------------------------------------------------------|
| SuperAdmin | Créer/activer/désactiver des entreprises (tenants)                    |
| Admin      | Paramétrer branding & règles RH, gérer employés/managers, types de congés, jours fériés |
| Manager    | Voir/approuver/refuser les demandes de son équipe                     |
| Employé    | Créer/consulter/annuler ses demandes de congé                         |

## Démarrage rapide

Prérequis : [.NET 8 SDK](https://dotnet.microsoft.com/download), SQL Server (local, Docker
ou Azure SQL).

```bash
# 1. Restaurer les packages
dotnet restore

# 2. Configurer la chaîne de connexion et le secret JWT
#    → src/GestionConges.API/appsettings.Development.json (à créer si besoin)
#    ou via dotnet user-secrets :
cd src/GestionConges.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GestionCongesDb;Trusted_Connection=True;TrustServerCertificate=true"
dotnet user-secrets set "Jwt:Secret" "une-cle-secrete-de-32-caracteres-minimum"

# 3. Créer la première migration EF Core (le DbContext existe déjà, la migration doit être générée)
dotnet tool install --global dotnet-ef   # si pas déjà installé
cd ../..
dotnet ef migrations add InitialCreate --project src/GestionConges.Infrastructure --startup-project src/GestionConges.API

# 4. Lancer l'API (applique automatiquement les migrations en Development)
dotnet run --project src/GestionConges.API
```

L'API démarre sur `https://localhost:5081` (ou `http://localhost:5080`), avec Swagger UI
disponible à la racine en environnement Development.

## Amorcer le premier SuperAdmin

Il n'y a volontairement aucun endpoint public pour créer un SuperAdmin (c'est le propriétaire
de la plateforme). Deux options :
1. Un script de **seed** exécuté une fois au démarrage (à ajouter dans `Program.cs`, exemple
   commenté à prévoir), ou
2. Une insertion manuelle en base avec un mot de passe haché via `BCrypt.Net.BCrypt.HashPassword(...)`.

## Principaux endpoints

| Méthode | Route                                    | Rôle requis     | Description |
|---------|-------------------------------------------|-----------------|--------------|
| POST    | `/api/auth/login`                         | Public          | Connexion, retourne un JWT |
| POST    | `/api/tenants`                            | SuperAdmin      | Créer une entreprise + son Admin |
| GET     | `/api/tenants`                            | SuperAdmin      | Lister les entreprises |
| PUT     | `/api/tenant-settings/branding`           | Admin           | Logo, couleurs |
| PUT     | `/api/tenant-settings/hr-rules`           | Admin           | Règles RH (jours ouvrés, report...) |
| POST    | `/api/employees`                          | Admin           | Créer un employé/manager |
| GET     | `/api/employees`                          | Admin, Manager  | Lister/filtrer les employés |
| POST    | `/api/leavetypes`                         | Admin           | Créer un type de congé |
| POST    | `/api/leaverequests`                      | Employé         | Créer une demande de congé |
| GET     | `/api/leaverequests/mine`                 | Employé         | Mes demandes |
| PATCH   | `/api/leaverequests/{id}/cancel`          | Employé         | Annuler une demande en attente |
| GET     | `/api/leaverequests/pending-for-me`       | Manager         | Demandes de mon équipe à traiter |
| PATCH   | `/api/leaverequests/{id}/process`         | Manager         | Approuver / refuser |
| GET     | `/api/dashboard/employee`                 | Employé         | Solde, demandes récentes |
| GET     | `/api/dashboard/manager`                  | Manager         | Vue équipe |
| GET     | `/api/dashboard/admin`                    | Admin           | Vue entreprise |
| GET     | `/api/dashboard/super-admin`              | SuperAdmin      | Vue plateforme |
| GET     | `/api/notifications`                      | Tous            | Mes notifications |
| POST    | `/api/holidays`                           | Admin           | Jours fériés de l'entreprise |

## Ce qui est couvert du backlog

Sprints 0 à 7 (setup, auth, employés, types de congés, demandes, validation, dashboards,
notifications) et une partie du Sprint 9 (branding + règles RH, jours fériés).

**Non implémenté dans cette version** (pistes d'extension, structure déjà prête pour les
accueillir) :
- Sprint 8 (vue calendrier) — les données (`LeaveRequest`, `Holiday`) sont déjà exposées via
  l'API, il reste à ajouter un endpoint d'agrégation par mois/équipe si besoin.
- Sprint 10 (export PDF/Excel, rapports) — non prioritaire pour un MVP backend.
- Envoi d'e-mails réel (les notifications sont pour l'instant uniquement en base / "in-app" ;
  `NotificationService.NotifyAsync` est le point d'accroche pour brancher un `IEmailSender`).

## Sécurité

- Mots de passe hachés avec BCrypt (work factor 12).
- Authentification par JWT (Bearer), expiration configurable (`Jwt:ExpiryHours`).
- Autorisation par rôle via des `AuthorizationPolicy` (`SuperAdminOnly`, `AdminOnly`,
  `ManagerOnly`, `AdminOrManager`, `AllTenantUsers`).
- Isolation multi-tenant systématique par `TenantId` dans chaque service.

## Remarque importante

Ce code a été écrit intégralement à la main dans un environnement sans SDK .NET disponible
pour compiler/tester (les dépôts nécessaires n'étaient pas accessibles). La structure, les
noms de types et la syntaxe C#/EF Core ont été vérifiés avec soin, mais il est recommandé de
lancer un `dotnet build` avant toute mise en production, pour rattraper une éventuelle
coquille de compilation.
