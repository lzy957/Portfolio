<%@ page language="java" import="java.util.*" pageEncoding="ISO-8859-1"%>
<%@ page language="java" import="dbset.*" %>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
  <head>
    <base href="<%=basePath%>">
    
    <title>My JSP 'SearchResult.jsp' starting page</title>
    
	<meta http-equiv="pragma" content="no-cache">
	<meta http-equiv="cache-control" content="no-cache">
	<meta http-equiv="expires" content="0">    
	<meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
	<meta http-equiv="description" content="This is my page">
	<!--
	<link rel="stylesheet" type="text/css" href="styles.css">
	-->
	<script type='text/javascript' src='http://www.openlayers.org/api/OpenLayers.js'></script>  <!--src最好指向自己机器上对应的js库 -->
    <script src="https://openlayers.org/en/v4.6.5/build/ol.js"></script>
 

  </head>
  
  <body onload='init();'>
    This is my JSP page. <br>
    <ul>
<li><p><b>name:</b>
<%= request.getParameter("name")%>

</p></li>
</ul>
<div style="top:50px; left: 20px; width: 1200px; height: 500px;">
		<div id='map' style='width:100%;height:100%;'>
		</div>
	</div>
  </body>

<script type = 'text/javascript'>
	 	
	function init()
	{	
<%
	SQLquery tj = new SQLquery();
        //String geojson=tj.getWKTByGid(1,"res1_4m");
        //String geojson = tj.getGeoJsonDisplay("res1_4m");
        String geojson = tj.getLocfromAtt(request.getParameter("name"));
        System.out.println(geojson);
 %>
	
	 var geojsonObject = <%= geojson %>;
	 var feature= (new ol.format.GeoJSON()).readFeatures(geojsonObject);
	 var vectorSource = new ol.source.Vector({
        features: feature
      });
     //vectorSource.addFeature(new ol.Feature(new ol.geom.Circle([5e6, 7e6], 1e6)));
     var vectorLayer = new ol.layer.Vector({
        source: vectorSource,
     
      });
	 var layers=[   
				
			new ol.layer.Tile({
		          source: new ol.source.OSM()	
		        }),
		    vectorLayer,
		    new ol.layer.Tile({      
		          source:new ol.source.TileWMS({      
		          url:'http://localhost:8080/geoserver/lzyMap/wms',      
		          params:{      
		                     'LAYERS':"lzyMap:res1_4m",
		                     'TILED':false      
		                 },      
		          serverType:'geoserver'
		       	  })     
		   })
      ];      
        
	var map = new ol.Map({
            // 设置地图图层
            layers: layers,
            // 设置显示地图的视图
            view: new ol.View({
              projection: 'EPSG:4326',
              center: [116.5, 39.5],
              zoom:4            // 并且定义地图显示层级为3
            }),
            // 让id为map的div作为地图的容器
            target: 'map'    
        });
	}
</script>
</html>