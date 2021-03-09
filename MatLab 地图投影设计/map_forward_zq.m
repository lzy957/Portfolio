function [ xi,yi ] = map_forward_zq(long ,lati)
%���в��γ�߶�Բ׶ͶӰ
%   �˴���ʾ��ϸ˵��
%����任��ʽ
%   lati γ�ȣ�long ����
%   x0��y0γ�߲���ֱ������
%   rho  �� E �� 

%�Ƕȱ任Ϊ����
%  long=long/180*pi;
%  lati=lati/180*pi;

% ��������ز���
R=637111600;
mu=14000000;

x0=(lati+0.06683225*lati^4)*R;
xn=x0+9.5493*lati;
yn=sqrt(112^2-xn^2)+20;
rho=(yn^2+(xn-x0)^2)/(2*(xn-x0));
y0=132/(210*180)*pi*1.1*(1-0.10096478*tan(long/5))*long;
Ep=asin(yn/rho);
E=Ep/180*1.1*(1-0.10096478*tan(long/5))*long;

%���x��y����
xi=rho+x0-rho*sin(E);
yi=y0+rho*sin(E);
end

