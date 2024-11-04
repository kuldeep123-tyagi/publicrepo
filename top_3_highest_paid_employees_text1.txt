WITH RankedEmployees AS (
    SELECT 
        EmployeeID, 
        Name, 
        Department, 
        Salary,
        ROW_NUMBER() OVER (PARTITION BY Department ORDER BY Salary DESC) AS SalaryRank
    FROM 
        Employees
)

SELECT 
    EmployeeID, 
    Name, 
    Department, 
    Salary
FROM 
    RankedEmployees
WHERE 
    SalaryRank <= 3
ORDER BY 
    Department, 
    Salary DESC;






Explanation of the Approach
Common Table Expression (CTE):

We define a CTE named RankedEmployees to first calculate the rank of employees within each department based on their salary.
The ROW_NUMBER() function is used here, which resets the numbering for each department (specified by PARTITION BY Department), and orders the employees in descending order of their salaries (specified by ORDER BY Salary DESC).
Filtering:

After ranking the employees, we select from the RankedEmployees CTE where the SalaryRank is less than or equal to 3. This means we only want the top 3 highest-paid employees per department.
Ordering the Results:

Finally, the results are ordered by department and salary in descending order to make it easy to view the top employees by department.