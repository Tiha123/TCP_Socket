using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using LitJson;

public class User_Info
{
    public string User_Name { get; private set; }
    public string User_Password { get; private set; }
    public string User_PhoneNum { get; private set; }
    public User_Info(string name, string password, string phone)
    {
        User_Name = name;
        User_Password = password;
        User_PhoneNum = phone;
    }
}
public class SQL_Manager : MonoBehaviour
{
    public User_Info info;
    public MySqlConnection con;//연결 관리
    public MySqlDataReader reader;//데이터 읽기, 사용 후 닫아야 다음 쿼리문이 동작함
    public string DB_path = string.Empty;
    public static SQL_Manager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DB_path = Application.dataPath + "/Database";
        string serverinfo = DBserverSet(DB_path);

        try
        {
            if (serverinfo.Equals(string.Empty))
            {
                Debug.Log("SQL Server Json Error");
                return;
            }
            con = new MySqlConnection(serverinfo);
            con.Open();
            Debug.Log("SQL server open complete!");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private string DBserverSet(string path)
    {
        if (File.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
        string jsonstring = File.ReadAllText(path + "/config.json");
        JsonData data = JsonMapper.ToObject(jsonstring);
        string serverinfo = $"Server={data[0]["IP"]};" + $"Database={data[0]["TableName"]};" + $"Uid={data[0]["ID"]};" + $"Pwd={data[0]["PW"]};" + $"Port={data[0]["PORT"]};" + "Charset=utf8;";
        return serverinfo;
    }

    private bool connection_check(MySqlConnection c)
    {
        if (c.State != System.Data.ConnectionState.Open)
        {
            c.Open();
            if (c.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
        }
        return true;
    }

    public bool Login(string id, string password)
    {
        try
        {
            if (!connection_check(con))
            {
                return false;
            }
            string sqlcommand = string.Format(@"SELECT User_Name,User_Password,User_PhoneNum FROM user_info WHERE User_Name ='{0}' AND User_Password='{1}';", id, password);
            MySqlCommand cmd = new MySqlCommand(sqlcommand, con);//쿼리문 DB 전송용 객체
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string name = (reader.IsDBNull(0)) ? string.Empty : reader["User_Name"].ToString();
                    string pwd = (reader.IsDBNull(1)) ? string.Empty : reader["User_Password"].ToString();
                    string pn = (reader.IsDBNull(2)) ? string.Empty : reader["User_PhoneNum"].ToString();
                    if (!name.Equals(string.Empty) || !pwd.Equals(string.Empty) || !pn.Equals(string.Empty))
                    {
                        info = new User_Info(name, pwd, pn);
                        if (reader.IsClosed == false)
                        {
                            reader.Close();
                            return true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (reader.IsClosed == false)
            {
                reader.Close();
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    }

    public bool Register(string id, string password, string phonenum)
    {
        if (!connection_check(con))
        {
            return false;
        }

        if (Check_Redundancy(id) == false)
        {
            string sqlcommand = string.Format(@"INSERT INTO user_info VALUES({0},{1},{2})", id, password, phonenum);
            MySqlCommand cmd = new MySqlCommand(sqlcommand, con);
            int a = cmd.ExecuteNonQuery();
            if (a > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.Log("ID already Exists!");
            return false;
        }
    }

    public bool DeleteAccount(string id, string password)
    {
        if (!connection_check(con))
        {
            return false;
        }

        if (Check_Redundancy(id, password) == true)
        {
            string sqlcommand = string.Format(@"DELETE FROM user_info WHERE User_Name={0} AND User_Password={1})", id, password);
            MySqlCommand cmd = new MySqlCommand(sqlcommand, con);
            int a = cmd.ExecuteNonQuery();
            if (a > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.Log("ID and Password doesn't Exists!");
            return false;
        }
    }

    public bool ChangePassword(string id, string password, string new_pwd)
    {
        if (!connection_check(con))
        {
            return false;
        }

        if (Check_Redundancy(id, password) == true)
        {
            string sqlcommand = string.Format(@"UPDATE user_info SET User_Password={0} WHERE User_Name={1} AND User_Password={2})", new_pwd, id, password);
            MySqlCommand cmd = new MySqlCommand(sqlcommand, con);
            int a = cmd.ExecuteNonQuery();
            if (a > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.Log("ID and Password doesn't Exists!");
            return false;
        }
    }

    private bool Check_Redundancy(string id)
    {
        string sqlcommand = string.Format(@"SELECT User_Name,User_Password,User_PhoneNum FROM user_info WHERE User_Name ='{0}';", id);
        MySqlCommand cmd = new MySqlCommand(sqlcommand, con);//쿼리문 DB 전송용 객체
        reader = cmd.ExecuteReader();
        if (reader.HasRows)
        {
            reader.Close();
            return true;
        }
        else
        {
            reader.Close();
            return false;
        }
    }

    private bool Check_Redundancy(string id, string password)
    {
        string sqlcommand = string.Format(@"SELECT User_Name,User_Password,User_PhoneNum FROM user_info WHERE User_Name ='{0}' AND User_Password = '{1}';", id, password);
        MySqlCommand cmd = new MySqlCommand(sqlcommand, con);//쿼리문 DB 전송용 객체
        reader = cmd.ExecuteReader();
        if (reader.HasRows)
        {
            reader.Close();
            return true;
        }
        else
        {
            reader.Close();
            return false;
        }
    }
}
