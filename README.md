# AuditEz

### *Make auditing easy.*

## Introduction

AuditEz is a library designed to simplify audit logging for enterprise applications.

It supports auditing create, update and delete operations for entities by automatically tracking changes throughout the entire object graph, including nested objects and collections.

## Prerequisites

AuditEz supports entities with properties of the following types:

- Primitive types
- Classes
- Lists of primitive types and classes

Recursive traversal is supported, allowing nested object graphs to be audited automatically.

The developer should provide:

- The entity (or entities) being audited
  - The entity identifier should be marked using the provided EntityIdentifier attribute.
- Id of the logged-in user
- Name of the calling process

There is a single Generate method which accepts either a CreateAuditLogRequest, UpdateAuditLogRequest, or DeleteAuditLogRequest, preventing invalid request combinations at compile time.

During validation, AuditEz verifies that the audited entity defines exactly one property marked with the EntityIdentifier attribute.

## Installation

=> *Currently out of scope.*

## Features

### Store audit logs

Data stored:

- Id of the user that performed the action,
- Name of the process that called the action,
- Type of action (create / update / delete),
- Id of the audit log,
- Id of the entity being logged,
- Type of the entity being logged,
- Originating Id of the entity that was logged (when auditing nested entities),
- Originating Type of the entity that was logged (when auditing nested entities),
- Properties affected:
  - Name of the property
  - Old value of the property
  - New value of the property

The library automatically discovers entity properties using reflection and recursively traverses nested objects and collections to capture changes throughout the entire entity graph.
    
### Automatic AuditLog table creation on installation

=> *Currently out of scope.*

### Display audit logs

=> *Currently out of scope.*
