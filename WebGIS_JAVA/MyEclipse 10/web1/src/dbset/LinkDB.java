package dbset;/**
 * 
 */

/**
 * @author Administrator
 *
 */

//import java.util.*;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class LinkDB {
	final private static  String driverClass="org.postgresql.Driver";// 驱动类
    final private static String url="jdbc:postgresql://localhost:5432/postgis_1";// 数据库地址
    final private static String username="Administrator";// 用户名
    final private static String password="postgre";// 密码
    public String getDriverClass() {
        return driverClass;
    }
    public String getUrl() {
        return url;
    }
    public String getUsername() {
        return username;
    }
    public String getPassword() {
        return password;
    }
    public static Connection getConn() {
         Connection conn = null ;
         try {
            Class.forName(driverClass);
            try {
                conn = DriverManager.getConnection(url, username, password);
            } catch (SQLException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            } 
        } catch (ClassNotFoundException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } 
        return conn;
        // TODO Auto-generated constructor stub
    }
    
    public static void main(String[] args) {
        Connection conn = LinkDB.getConn();
        if(conn!=null){
            System.out.println("数据库连接成功！");
        }
    }
}
