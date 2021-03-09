package dbset;/**
 * 
 */

import java.sql.Connection;
//import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;  
//import java.sql.DriverManager;  
import java.sql.PreparedStatement;    

import org.postgresql.geometric.PGcircle;
import org.postgresql.geometric.PGpoint;


//import org.postgresql.geometric.*;

/**
 * @author Administrator
 *
 */
public class SQLquery {
	
	  /** 根据gid获取对应表中的geom 的WKT信息
     * @param gid  主键
     * @param tablename 表名
     * @return WKT描述的几何对象
     */
    public String getWKTByGid(int gid,String tablename){
        Connection conn = LinkDB.getConn();
        Statement stmt = null;
        ResultSet rs = null;
        String wktString = null;
        String sql = "select ST_AsGeoJson(geom) from res1_4m where gid=1 ";
        if(conn!=null){
            try {
                stmt = conn.createStatement();  
                rs = stmt.executeQuery(sql);  
                if(rs.next()){
                    wktString =  rs.getString(1);
                }
            } catch (SQLException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }finally {
                try {
                    if(rs!=null){
                        rs.close();
                    }
                    if(stmt!=null){
                        stmt.close();
                    }
                    if(conn!=null){
                        conn.close();
                    }
                } catch (SQLException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }
        return wktString ;
    }
    public String getGeoJsonDisplay(String tablename){
        Connection conn = LinkDB.getConn();
        Statement stmt = null;
        ResultSet rs = null;
        String wktString = "{\"type\":\"FeatureCollection\",\"features\":[";
        String sql = "select *,ST_AsGeoJson(geom) from res1_4m";
        if(conn!=null){
            try {
                stmt = conn.createStatement();  
                rs = stmt.executeQuery(sql);
                if(rs.next()){
                	//rs.getArray(11);
                	wktString = wktString+"{\"type\":\"Feature\",\"id\":";
                	wktString = wktString+"\""+rs.getString(1)+"\",\"properties\":{\"name\":\""+rs.getString(11)+"\",\"adclass\":"+rs.getString(10)+"},\"geometry\":"+rs.getString(14)+"}";
                    //wktString =  wktString+rs.getString(1);
                    //System.out.println(rs.getArray(13));
                }
                while(rs.next())
                {
                	wktString = wktString+",{\"type\":\"Feature\",\"id\":";
                	wktString = wktString+"\""+rs.getString(1)+"\",\"properties\":{\"name\":\""+rs.getString(11)+"\",\"adclass\":"+rs.getString(10)+"},\"geometry\":"+rs.getString(14)+"}";
                	//System.out.println(rs.getString(13));
                	//wktString=wktString+","+rs.getString(1);
                }
                wktString=wktString+"]}";
            } catch (SQLException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }finally {
                try {
                    if(rs!=null){
                        rs.close();
                    }
                    if(stmt!=null){
                        stmt.close();
                    }
                    if(conn!=null){
                        conn.close();
                    }
                } catch (SQLException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }
        return wktString ;
    }
    
    public void insertField(){
    	Connection conn = LinkDB.getConn();
    	Statement stmt = null;
        ResultSet rs = null;
        String sql = "ALTER TABLE res1_4m ADD COLUMN image bytea;";
        if(conn!=null){
            try{
                stmt = conn.createStatement();  
                rs = stmt.executeQuery(sql);}
            catch (SQLException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }finally {
                try {
                    if(rs!=null){
                        rs.close();
                    }
                    if(stmt!=null){
                        stmt.close();
                    }
                    if(conn!=null){
                        conn.close();
                    }
                } catch (SQLException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }
    }
    
    public void insertCircle() throws SQLException {  
    	 Connection conn = LinkDB.getConn(); 
        PGpoint center = new PGpoint(1, 2.5);  
        // PGpolygon polygon = new PGpolygon(points);  
        double radius = 4;  
        PGcircle circle = new PGcircle(center, radius);  
  
        PreparedStatement ps = conn.prepareStatement("INSERT INTO geomtest(mycirc) VALUES (?)");  
        ps.setObject(1, circle);  
        ps.executeUpdate();  
        ps.close();  
    }  
  
    public String getLocfromAtt(String Name){
    	Connection conn = LinkDB.getConn();
        Statement stmt = null;
        ResultSet rs = null;
        String wktString = null;
        String sql = "select *,ST_AsGeoJson(geom) from res1_4m where pinyin="+"'"+Name+"'";
        if(conn!=null){
            try {
                stmt = conn.createStatement();  
                rs = stmt.executeQuery(sql);  
                if(rs.next()){
                    wktString =  rs.getString(14);
                }
            } catch (SQLException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }finally {
                try {
                    if(rs!=null){
                        rs.close();
                    }
                    if(stmt!=null){
                        stmt.close();
                    }
                    if(conn!=null){
                        conn.close();
                    }
                } catch (SQLException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }
        return wktString ;
    }
    
    public void retrieveCircle() throws SQLException {  
    	Connection conn = LinkDB.getConn();
    	Statement stmt = conn.createStatement();  
        ResultSet rs = stmt.executeQuery("SELECT mycirc, area(mycirc) FROM geomtest");  
        rs.next();  
        PGcircle circle = (PGcircle) rs.getObject(1);  
        double area = rs.getDouble(2);  
        // PG  
  
        PGpoint center = circle.center;  
        double radius = circle.radius;  
  
        System.out.println("Center (X, Y) = (" + center.x + ", " + center.y + ")");  
        System.out.println("Radius = " + radius);  
        System.out.println("Area = " + area);  
    } 
    
    public static void main(String[] args) {
    	SQLquery tj = new SQLquery();
        //System.out.println(tj.getWKTByGid(1, "res1_4m"));
        //System.out.println(tj.getGeoJsonDisplay("res1_4m"));
    	//tj.insertField();
        System.out.println(tj.getLocfromAtt("Beijing"));
    }

}
