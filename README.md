# StackOverflow Tags API

## Overview
This project is an API that provides information about StackOverflow tags. It allows users to retrieve, analyze, and manage tags used in StackOverflow questions.

## Features
- Retrieve a list of fetched tags
- Fetch a limited number of tags
- Search for tags by name or share rate

## Installation
To install and run the project locally, follow these steps:

1. Clone the repository:
   git clone https://github.com/jakubGryzio/StackOverflowTagsApi.git
   cd StackOverflowTagsApi

The API will be available at `http://localhost:5000` and `https://localhost:5050`.

## Usage
### Get All Tags
```bash
GET /api/Tags?PageNumber=<number>&PageSize=<number>&Order=<asc|desc>&SortBy=<name|share>
POST /api/FetchTags?limit=<number between 1 and 10000>
