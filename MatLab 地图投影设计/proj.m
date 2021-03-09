S=shaperead('landareas.shp'); %读取shapefile文件
Slength=length(S); %读取属性长度
for cou =1:Slength
   xlength=length(S(cou,1).X); 
    for count=1:xlength
        long=S(cou,1).X(count);
        lati=S(cou,1).Y(count); %读取某一属性的经纬坐标。
%         if long<=-30
%             long=long+210;
%         else
%             long=long-150;
%         end        %输出中央经线为东经150的投影式的经度变化
%         [xi,yi]=map_forword_rad(long,lati);  %遍历该shapefile文件上的每一点，
        %通过逐点变化，最终生成给投影下的地图数据。
         [xi,yi]=map_forward_zq(long,lati); 
        S(cou,1).X(count)=yi;
        S(cou,1).Y(count)=xi;
    end
end
mapshow(S); %显示投影变换够的地图数据
%shapewrite(S,'world_map_conversation.shp'); %导出地图数据
