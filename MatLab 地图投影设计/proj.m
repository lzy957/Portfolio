S=shaperead('landareas.shp'); %��ȡshapefile�ļ�
Slength=length(S); %��ȡ���Գ���
for cou =1:Slength
   xlength=length(S(cou,1).X); 
    for count=1:xlength
        long=S(cou,1).X(count);
        lati=S(cou,1).Y(count); %��ȡĳһ���Եľ�γ���ꡣ
%         if long<=-30
%             long=long+210;
%         else
%             long=long-150;
%         end        %������뾭��Ϊ����150��ͶӰʽ�ľ��ȱ仯
%         [xi,yi]=map_forword_rad(long,lati);  %������shapefile�ļ��ϵ�ÿһ�㣬
        %ͨ�����仯���������ɸ�ͶӰ�µĵ�ͼ���ݡ�
         [xi,yi]=map_forward_zq(long,lati); 
        S(cou,1).X(count)=yi;
        S(cou,1).Y(count)=xi;
    end
end
mapshow(S); %��ʾͶӰ�任���ĵ�ͼ����
%shapewrite(S,'world_map_conversation.shp'); %������ͼ����
