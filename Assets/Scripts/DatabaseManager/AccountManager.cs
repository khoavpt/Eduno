/*
AccountManager: Responsible for handling login and register operations.

Constructors: 
- public AccountManager(string databasePath):
    + Parameters: 
        - databasePath (string): The path to the SQLite database file.

Methods:
- public int VerifyLogin(string username, string password): Verifies the provided username and password.
    + Parameters: 
        - username (string)
        - password (string)
    + Return value:
        - AccountManager.MISSING_VALUES (-1) if either the username or password was not submitted.
        - AccountManager.FAILURE (0) if there's no account with the provided username and password.
        - AccountManager.SUCCESS (1) if there's already an account with the provided username and password.

- public int CreateAccount(string username, string password): Creates a new user account with the provided username and password.
    + Parameters: 
        - username (string)
        - password (string)
    + Return value:
        - AccountManager.MISSING_VALUES (-1) if either the username or password was not submitted.
        - AccountManager.FAILURE (0) if there's already an account with the provided username.
        - AccountManager.SUCCESS (1) if there's no account with the provided username and password.
*/
using System.Data.SQLite;

public class AccountManager
{   
    private string connectionString;

    public static int MISSING_VALUES = -1;
    public static int FAILURE = 0;
    public static int SUCCESS = 1;

    public AccountManager(string databasePath)
    {
         connectionString = string.Format("Data Source={0};Version=3;", databasePath);
    }

    public int VerifyLogin(string username, string password)
    {
        if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return AccountManager.MISSING_VALUES;
        }

        // Check if there's an account with the provided username and password
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteCommand checkCmd = new SQLiteCommand(connection))
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                checkCmd.Parameters.AddWithValue("@username", username);
                checkCmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count <= 0)
                {
                    // If there's no account with the provided username and password, login failed
                    return AccountManager.FAILURE;
                }
            }
        }
        // If there's an account with the provided username and password, login successful
        return AccountManager.SUCCESS;
    }


    public int CreateAccount(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return AccountManager.MISSING_VALUES;
        }

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Check if an account with the provided username already exists
            using (SQLiteCommand checkCmd = new SQLiteCommand(connection))
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
                checkCmd.Parameters.AddWithValue("@username", username);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    // If an account with the provided username already exists, register failed
                    return AccountManager.FAILURE;
                }
            }

            // Else, create a new account with the provided username and password
            using (SQLiteCommand createCmd = new SQLiteCommand(connection))
            {
                createCmd.CommandText = "INSERT INTO users (username, password, points) VALUES (@username, @password, @points)";
                createCmd.Parameters.AddWithValue("@username", username);
                createCmd.Parameters.AddWithValue("@password", password);
                createCmd.Parameters.AddWithValue("@points", 0);

                createCmd.ExecuteNonQuery();
            }
        }

        // Account creation successful
        return AccountManager.SUCCESS;
    }
}