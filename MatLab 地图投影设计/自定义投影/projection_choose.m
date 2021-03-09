landareas=shaperead('landareas.shp','UseGeoCoords',true);
axesm('quartic_sanson','Frame','on','Grid', 'on','Origin',[0 0 0]);
 tissot();
mdistort;
geoshow(landareas);

%
% list(i).IdString       = 'equalpolycon_candy';
% list(i).Name           = 'PolyconC';
% 
% 
% list(i).IdString       = 'equalpolycon_lati0';
% list(i).Name           = 'PolyconLZERO';
% 
% list(i).IdString       = 'equalpolycon_pole_arc';
% list(i).Name           = 'PolyconAP';
% 
% list(i).IdString       = 'equalpolycon_pole_straightline';

% list(i).IdString       = 'equalpolycon_polesetting';
% list(i).Name           = 'PolyconicSetPole';