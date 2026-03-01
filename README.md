# AuditEz

### *Make auditing easy.*

## Introduction

AuditEz is a library aimed to make audit logging easy for existing projects.

It supports auditing for create, update and delete operations.

## Prerequisites

AuditEz supports objects with properties of the following types:
- primitive types, 
- classes, 
- lists of the before stated types.

The developer should provide:
- id of the objects, 
- id of the user the logged-in user,
- name of the calling process, 
- type of action (create / update / delete)

## Installation

=> *Currently out of scope.*

## Features

### Store audit logs

Data stored:

- Id of the user that performed the action,
- Name of the process that called the action,
- Type of action (create / update / delete)
- Id of the audit log
- Id of the entity that was logged
- Type of the entity that was logged
- Properties affected:
  - Name of the property
  - Old value of the property
  - New value of the property

    
### Automatic AuditLog table creation on installation

=> *Currently out of scope.*

### Display audit logs

=> *Currently out of scope.*


