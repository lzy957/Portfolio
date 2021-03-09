<%@ page language="java" import="java.util.*" pageEncoding="ISO-8859-1"%>
<%@ page language="java" import="dbset.*" %>

<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html lang="en">
  <head>
    <base href="<%=basePath%>">
    
    <title>My JSP 'MyJsp.jsp' starting page</title>
    
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
 	<style>
      .map {
        height: 400px;
        width: 100%;
      }
      .ol-popup {
        position: absolute;
        background-color: white;
        -webkit-filter: drop-shadow(0 1px 4px rgba(0,0,0,0.2));
        filter: drop-shadow(0 1px 4px rgba(0,0,0,0.2));
        padding: 15px;
        border-radius: 10px;
        border: 1px solid #cccccc;
        bottom: 12px;
        left: -50px;
      }
      .ol-popup:after, .ol-popup:before {
        top: 100%;
        border: solid transparent;
        content: " ";
        height: 0;
        width: 0;
        position: absolute;
        pointer-events: none;
      }
      .ol-popup:after {
        border-top-color: white;
        border-width: 10px;
        left: 48px;
        margin-left: -10px;
      }
      .ol-popup:before {
        border-top-color: #cccccc;
        border-width: 11px;
        left: 48px;
        margin-left: -11px;
      }
      .ol-popup-closer {
        text-decoration: none;
        position: absolute;
        top: 2px;
        right: 8px;
      }
      .ol-popup-closer:after {
        content: "x";
      }
 	</style>
  </head>
  
  <body onload='init();'>
    This is my JSP page. <br>
    <div style="top:50px; left: 20px; width: 1200px; height: 500px;">
		<div id='map' style='width:100%;height:100%;'>
			<div id="popup" class="ol-popup">
		    <a href="#" id="popup-closer" class="ol-popup-closer"></a>
		    <div id="popup-content" style="width:300px; height:120px;"></div>
		</div>
			<form class="form-inline">
      <label>Action type &nbsp;</label>
        <select id="type" class="form-control">
          <option value="click" selected>Click</option>
          <option value="singleclick">Single-click</option>
          <option value="pointermove">Hover</option>
          <option value="altclick">Alt+Click</option>
          <option value="none">None</option>
        </select>
      <span id="status">&nbsp;0 selected features</span>
    </form>
			<div id="search_group" class="search_group">
			<form action="SearchResult.jsp" method="Post">
				<input type="text" name="name">
	        	<input type="submit" value="update" />
			</form>
			</div>
		</div>		
	</div>
  </body>
  
  <script type = 'text/javascript'>
   //import Select from 'ol/interaction/Select.js';
	var select_control = null;    // SelectFeature Control	
	function init()
	{	
	<%
    	SQLquery tj = new SQLquery();
        //String geojson=tj.getWKTByGid(1,"res1_4m");
        String geojson = tj.getGeoJsonDisplay("res1_4m");
        System.out.println(geojson);
	%>
	
	 var geojsonObject = <%= geojson %>;
	 var feature= (new ol.format.GeoJSON()).readFeatures(geojsonObject);
	 var style = new ol.style.Style({
	 	radius:10,  
  		fill: new ol.style.Fill({ //矢量图层填充颜色，以及透明度  
    	color: 'rgba(0, 255, 255, 0.5)'  
 	 }),  
  	stroke: new ol.style.Stroke({ //边界样式  
    	color: '#319FD3',  
    	width: 1  
 	 }), 
 	text: new ol.style.Text({ //文本样式  
    	font: '12px Calibri,sans-serif',  
    	fill: new ol.style.Fill({  
      	color: '#000'  
    }), 
    stroke: new ol.style.Stroke({  
      color: '#fff',  
      width: 3  
    })  
  })  
}); 
//feature.setStyle(style);
	 var vectorSource = new ol.source.Vector({
        features: (new ol.format.GeoJSON()).readFeatures(geojsonObject)
        //features:feature
      });
     //vectorSource.addFeature(new ol.Feature(new ol.geom.Circle([5e6, 7e6], 1e6)));
     var vectorLayer = new ol.layer.Vector({
        source: vectorSource,
        //style: style,
     	//renderers: renderer
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
              zoom: 4            // 并且定义地图显示层级为3
            }),
            // 让id为map的div作为地图的容器
            target: 'map'    
        });
        
        var container = document.getElementById('popup');
        var content = document.getElementById('popup-content');
        var closer = document.getElementById('popup-closer');
 
		 
		/**
		 * Add a click handler to hide the popup.
		 * @return {boolean} Don't follow the href.
		 */
		closer.onclick = function() {
		  overlay.setPosition(undefined);
		  closer.blur();
		  return false;
		  }
		        
		var overlay = new ol.Overlay(/** @type {olx.OverlayOptions} */ ({
		  element: container,
		  autoPan: true,
		  autoPanAnimation: {
		    duration: 250   //当Popup超出地图边界时，为了Popup全部可见，地图移动的速度. 
		  }
		}));
		        
       // vectorLayer.events.register("click",vectorLayer,OnVLayerClick);
       //addSelectControl(map, vectorLayer);
       let select=null;  // ref to currently selected interaction

      // select interaction working on "singleclick"
      const selectSingleClick = new ol.interaction.Select();

      // select interaction working on "click"
      const selectClick = new ol.interaction.Select({
        condition: ol.events.condition.click
      });

      // select interaction working on "pointermove"
      const selectPointerMove = new ol.interaction.Select({
        condition: ol.events.condition.pointerMove
      });

      const selectAltClick = new ol.interaction.Select({
        condition: function(mapBrowserEvent) {
          return ol.events.condition.click(mapBrowserEvent) && ol.events.condition.altKeyOnly(mapBrowserEvent);
        }
      });

      const selectElement = document.getElementById('type');

      const changeInteraction = function() {
        if (select !== null) {
          map.removeInteraction(select);
        }
        const value = selectElement.value;
        if (value == 'singleclick') {
          select = selectSingleClick;
        } else if (value == 'click') {
          select = selectClick;
        } else if (value == 'pointermove') {
          select = selectPointerMove;
        } else if (value == 'altclick') {
          select = selectAltClick;
        } else {
          select = null;
        }
        if (select !== null) {
          map.addInteraction(select);
          select.on('select', function(e) {
           var coordinate = e.coordinate;
           var featurehl=e.target.getFeatures();
          var array=featurehl.getArray();
          var geometry=array[0].getGeometry(); 
          console.log(array[0]); 
          document.getElementById('status').innerHTML = '&nbsp;' +
                e.target.getFeatures().getLength() +
                ' selected features (last operation selected ' + e.selected.length +
                ' and deselected ' + e.deselected.length + ' features)'+geometry.getCoordinates();
          //var coordinate = e.coordinate;
		  //var hdms = ol.coordinate.toStringHDMS(ol.proj.transform(
		  //    coordinate, 'EPSG:3857', 'EPSG:4326'));
		  content.innerHTML = '<p>Name  '+array[0].N.name+'    ID '+array[0].getId()+'</p><p>Coordinate</p><code>' + geometry.getCoordinates() + '</code>';
		  overlay.setPosition(geometry.getCoordinates());
		  map.addOverlay(overlay);         
          /*
                popup = new OpenLayers.Popup.FramedCloud("chicken", 
                                 geometry.getCoordinates(),
                                  null,
                                  "<div style='font-size:.8em'>Feature: " + e.selected.id +"<br />Area: " + e.selected.name+"</div>",
                                  null, true, onPopupClose);
         feature.popup = popup;
         map.addPopup(popup);*/
          });
        }
      };


      /**
       * onchange callback on the select element.
       */
      selectElement.onchange = changeInteraction;
      changeInteraction();
       	}
	
	 function addSelectControl(map, vectorLayer)
      {
          if(select_control!=null)
          {
              return ;
          }
          alert("addSelectControl");
          select_control = new ol.Control.SelectFeature(vectorLayer,
                                                           {
                                                              hover: false,
                                                              onSelect: onFeatureSelect, 
                                                              onUnselect: onFeatureUnselect
                                                            });
          map.addControl(select_control);
          select_control.activate();
      }
      
      // Feature 选中事件响应
     function onFeatureSelect(feature) 
     {
         selectedFeature = feature;
         popup = new OpenLayers.Popup.FramedCloud("chicken", 
                                  feature.geometry.getBounds().getCenterLonLat(),
                                  null,
                                  "<div style='font-size:.8em'>Feature: " + feature.id +"<br />Area: " + feature.geometry.getArea()+"</div>",
                                  null, true, onPopupClose);
         feature.popup = popup;
         map.addPopup(popup);
         
     }
 
     // Feature取消选中事件响应
     function onFeatureUnselect(feature) 
     {
         map.removePopup(feature.popup);
         feature.popup.destroy();
         feature.popup = null;
     }    
 
     function onPopupClose(evt) {
         select_control.unselect(selectedFeature);
     }
	/*function  sub(){  
       $.ajax({  
       type:"GET",  
       url:"/ajax/Jajax",  
       data:{name:$("#search_input").val()},  
       statusCode: {404: function() {  
            alert('page not found'); }  
         },      
       success:function(data,textStatus){  
       $("#txtHint").html(data);  
       }  
       });  
     }  */
   
    
			
function theSearch(){
if(document.getElementById("search_input").value != "" ){
	var tempname=document.getElementById("search_input").value;
       
    	//SQLquery tj1 = new SQLquery();
        //String geojson=tj.getWKTByGid(1,"res1_4m");
        var geojson1 = tj1.getLocfromAtt("Bejing");
        //System.out.println(geojson1);
        
	//var geojsonObject1 = geojson1;
/*	var feature = (new ol.format.GeoJSON()).readFeatures(geojson1);
	 var highlightLayer  = new ol.layer.Vector({
       source: highllightSource,
    });
 var highllightSource = new ol.source.Vector({
 		features: feature,
 });
	
	// 通过 ol.color.asArray 将原来16进制的颜色值，改为 r,g,b,a的数组
 var highAlpColor = ol.color.asArray('#1fca04');
 highAlpColor = highAlpColor.slice();  
 highAlpColor[3] = 0.9;

 // 分开来设置 style
 	feature.setStyle(new ol.style.Style({
     	image: new ol.style.Circle({
         	radius: 7,
         	fill: new ol.style.Fill({
            color: highAlpColor
         })
     })
 }));    */
        }	
}				
</script>
  
</html>
