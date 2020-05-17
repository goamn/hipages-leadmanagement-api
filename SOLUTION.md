# hipages-leadmanagement-api

-Separated API and UI into two solutions. Upgraded provided MySQL script into PostgreSQL script.
-Separated production intended SQL script from sample data SQL statements. Opened endpoint to allow sample data to be populated in database (PopulateWithTestData). Would like to change that to an automated behavior based on whether the solution was run locally or not.
-Would also like to change the (PopulateWithTestData) behavior to drop database data or at least sample data. Also remove explicit id's in INSERT statements and make tests depend on something else when testing test data.
-UpdateLead controller action should be PUT only. Enabled POST as there was a lot of trouble getting axios.put(..) to work with a single string query parameter (seems to be a common problem), used POST as a workaround.
