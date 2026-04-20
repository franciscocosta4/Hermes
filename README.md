# Hermes

Hermes is a personal finances manager for the web made in .net. It allows users to track incomes and expenses, categorize them, and get insights about spending behavior. The primary goal is to help users monitor if they are overspending and provide summary and forecast reports based on historical data.

## Stack
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)    ![Postgres](https://img.shields.io/badge/postgres-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)  ![CSS](https://img.shields.io/badge/css-%23663399.svg?style=for-the-badge&logo=css&logoColor=white) 

---

## Features

### Core Features
* Add, update, and delete expenses (description, amount, date, category)
* Record incomes (salary or other receipts)
* List all expenses and incomes
* View total expenses, total income, and balance for the last 30 days
* Calculate average expenses over the last 90 days
* Detect overspending (compare current month to 90-day average)

### Optional / Future Features
* Categories for expenses
* Filter expenses by category
* Monthly budget tracking
* Alerts when exceeding budget or spending more than the average
* Export expenses to CSV
* Goal-based analysis (e.g., "Can I afford this purchase?")

---

## Database Design

The database supports multiple users and ensures that all data is user-specific.
<img width="861" height="799" alt="image" src="https://github.com/user-attachments/assets/26e6a980-243a-42f7-bbe1-949a2058729a" />

---

## Reports and Calculations

* **Last 30 days expenses / income / balance**
* **90-day average expenses**
* **Overspending detection**: Compare last 30 days spending with 90-day average
* **Optional insights**: Predict whether a user can afford a purchase based on their historical spending behavior

---

## License:
[![Licence](https://img.shields.io/github/license/Ileriayo/markdown-badges?style=for-the-badge)](./LICENSE)
