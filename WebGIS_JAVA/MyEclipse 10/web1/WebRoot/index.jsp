<%@ page language="java" import="java.util.*" pageEncoding="ISO-8859-1"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
  <head>
    <base href="<%=basePath%>">
    
    <title>My JSP 'index.jsp' starting page</title>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
	<meta http-equiv="pragma" content="no-cache">
	<meta http-equiv="cache-control" content="no-cache">
	<meta http-equiv="expires" content="0">    
	<meta http-equiv="keywords" content="keyword1,keyword2,keyword3">
	<meta http-equiv="description" content="This is my page">
	<!--
	<link rel="stylesheet" type="text/css" href="styles.css">
	-->
	<script type='text/javascript' src='http://www.openlayers.org/api/OpenLayers.js'></script>  <!--src最好指向自己机器上对应的js库 -->
	<script type = 'text/javascript'>
		var map;
		//var geographic1 = new OpenLayers.Projection("EPSG:4326");
		//var geographic2 = new OpenLayers.Projection("EPSG:404000");
		
		function init()
		{
			//定义地图边界
			//var usbounds= new OpenLayers.Bounds(73.44696044921875,3.408477306365967,135.08583068847656,53.557926177978516).transform( geographic1, geographic2 );  //设置坐标范围对象
			var bounds= new OpenLayers.Bounds(73.44696044921875,3.408477306365967,135.08583068847656,53.557926177978516);
			var options = {				
				projection: "geographic1",		//地图投影方式
				maxExtent:bounds,				     //坐标范围
				units:'degrees'	,        //单位
				//uscenter: new OpenLayers.LonLat(116.5, 39.5).transform( geographic1, geographic2 ),   //图形中心坐标
				center: new OpenLayers.LonLat(116.5, 39.5),
			};
			map = new OpenLayers.Map('map_v',options);     //构建一个地图对象，并指向后面页面中的div对象，这里是'map'
			
			var wms = new OpenLayers.Layer.WMS(    //构建地图服务WMS对象，
			  "Map Of China",         //图层名称，最好用中文，由于页面编码原因，写中文可能乱码，可以到网上搜索解决方法			
				"http://localhost:8080/geoserver/lzyMap/wms", 		 	//geoserver所在服务器地址及对应的地图服务		
				{                                           //以下是具体访问参数
					layers: "lzyMap:bou2_4p",  //图层名称，对应与我们自己创建的服务layers层名
					style:'',            //样式
					format:'image/png',   //图片格式
					TRANSPARENT:"true",   //是否透明
				},
				  {isBaseLayer: true}   //是否基础层，必须设置
				);
			//添加wms图层
			
			//var osm = new OpenLayers.Layer.OSM();  
			var wms1 = new OpenLayers.Layer.WMS(
			"map of cities",
			"http://localhost:8080/geoserver/lzyMap/wms",
			{
				layers:"lzyMap:res1_4m",
				style:'',            //样式
				format:'image/png',   //图片格式
				TRANSPARENT:"true",   //是否透明
			},
			{isBaseLayer: true}
			 //{opacity: 0.5}
			 );
			//map.addLayer(wms);	//增加这个wms图层到map对象
			map.addLayer(wms1);
			//map.addLayer(osm);		
			//添加control空间			
	   map.addControl(new OpenLayers.Control.LayerSwitcher());  //增加图层控制
       map.addControl(new OpenLayers.Control.MousePosition());  //增加鼠标移动显示坐标
       
       map.zoomToExtent(bounds);		//缩放到全图显示
		}		
						
	</script>
  </head>
  
  <body onload='init();'>
	<div style="top:50px; left: 20px; width: 1200px; height: 500px;">
		<div id='map_v' style='width:100%;height:100%;'>
		</div>
	</div>
    This is my JSP page. <br>
  </body>
</html>
