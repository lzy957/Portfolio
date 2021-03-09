function [ xi,yi ] = map_forward_zq(long ,lati)
%正切差分纬线多圆锥投影
%   此处显示详细说明
%正解变换公式
%   lati 纬度；long 经度
%   x0，y0纬线部分直角坐标
%   rho  ρ E δ 

%角度变换为弧度
%  long=long/180*pi;
%  lati=lati/180*pi;

% 椭球体相关参数
R=637111600;
mu=14000000;

x0=(lati+0.06683225*lati^4)*R;
xn=x0+9.5493*lati;
yn=sqrt(112^2-xn^2)+20;
rho=(yn^2+(xn-x0)^2)/(2*(xn-x0));
y0=132/(210*180)*pi*1.1*(1-0.10096478*tan(long/5))*long;
Ep=asin(yn/rho);
E=Ep/180*1.1*(1-0.10096478*tan(long/5))*long;

%求得x、y坐标
xi=rho+x0-rho*sin(E);
yi=y0+rho*sin(E);
end

