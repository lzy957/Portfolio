<%@ page language="java" import="java.util.*" pageEncoding="ISO-8859-1"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
  <head>
    <base href="<%=basePath%>">
    
    <title>My JSP 'osm_p.jsp' starting page</title>
     <meta http-equiv="Content-Type" content="text/html; charset=gb2312">
    
	<meta http-equiv="pragma" content="no-cache">
	<meta http-equiv="cache-control" content="no-cache">
	<meta http-equiv="expires" content="0">    
	<meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
	<meta http-equiv="description" content="This is my page">
	<!--
	<link rel="stylesheet" type="text/css" href="styles.css">
	-->	  
    <script type='text/javascript' src='http://www.openlayers.org/api/OpenLayers.js'></script>

</head>

 <body>
 This is my JSP page. <br>
    <div id="map">
        <script>
            var geographic = new OpenLayers.Projection("EPSG:4326");
            var mercator = new OpenLayers.Projection("EPSG:900913");

            var usBounds = new OpenLayers.Bounds(87.60611724853516,20.03179359436035,126.64334106445312,45.741493225097656).transform( geographic, mercator );
            var usCenter = new OpenLayers.LonLat(116.5, 39.5).transform( geographic, mercator );

            var options = {
                projection: geographic,	
                displayProjection: geographic,
                units: "degrees",
                maxExtent:usBounds,
                center: usCenter,
                //maxResolution: 15654.3399
            };

            var map = new OpenLayers.Map("map",options);

            var osm = new OpenLayers.Layer.OSM();            

            var wms = new OpenLayers.Layer.WMS(
                "China Map",
                "http://localhost:8080/geoserver/lzyMap/wms",
                {
                   layers: 'lzyMap:res1_4m',
                   styles: '',     
                   format: 'image/png',
                   transparent: true
                },
                {
                   opacity: 0.5 //, 'isBaseLayer': true
                }
            );
            
            map.addLayers([osm,wms]);  
            //map.addLayer(wms);  
            //map.addLayer(osm); 
            
            map.addControl(new OpenLayers.Control.LayerSwitcher());
            //zeigt die Koordinaten der aktuellen Mause-Position an
            map.addControl(new OpenLayers.Control.MousePosition());
            map.zoomToExtent(usBounds);
            
        </script>
    </div>
 </body>
</html>
