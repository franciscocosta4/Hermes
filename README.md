# Hermes

Hermes is a personal finances manager for the web made in .net. It allows users to track incomes and expenses, categorize them, and get insights about spending behavior. The primary goal is to help users monitor if they are overspending and provide summary and forecast reports based on historical data.
<img width="1902" height="887" alt="image" src="https://github.com/user-attachments/assets/8f19b5ea-7648-4fe3-8d24-b8d3355e85ce" />
<img width="1901" height="885" alt="image" src="https://github.com/user-attachments/assets/4068ad99-5a76-4166-a0ec-51e9ba34eb5a" />

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
<img width="983" height="547" alt="image" src="https://github.com/user-attachments/assets/326ad21c-487c-4062-b754-8dced556562b" />

### budget
Defines monthly spending limits per user.

- Each record represents a budget for a specific month and year
- Used to detect overspending
- Helps users control financial limits

### savings_goal
Represents financial goals set by the user (e.g. saving for a car, vacation, etc.).

- `target_amount` → goal value
- `current_amount` → progress made so far
- `deadline` → optional target completion date
- Helps users track long-term savings objectives

### alert
Stores system-generated notifications for users.

- Used for:
  - budget exceeded warnings
  - overspending detection
  - financial insights

- `is_read` tracks whether the user has seen the alert

---

## Reports and Calculations

* **Last 30 days expenses / income / balance**
* **90-day average expenses**
* **Overspending detection**: Compare last 30 days spending with 90-day average
* **Optional insights**: Predict whether a user can afford a purchase based on their historical spending behavior

---

## License:
[![Licence](https://img.shields.io/github/license/Ileriayo/markdown-badges?style=for-the-badge)](./LICENSE)
