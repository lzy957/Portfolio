function [ xi,yi ] = map_forword( long,lati)
%�Ȳ��γ�߶�Բ׶ͶӰ ����任��ʽ
%   �˴���ʾ��ϸ˵��
%   lati γ�ȣ�long ����
%   x0��y0γ�߲���ֱ������
%   rho  �� delta �� 

%��150�Ⱦ���Ϊ���뾭��
if long>-30&&long<=180
    long=long-150;
elseif long>=-180&&long<=-30
    long=long+210;
else
    long=NaN;
end

%�Ƕȱ任Ϊ����
% long=long/180*pi;
% lati=lati/180*pi;

%��ʽ���� ����Ե�����ϵ�ֱ������
x0=1.9499079012403*10^(-5)*lati^3+4.79161212198753*lati;
S=x0;
%b=1/(1-C*180);
b1=1.1;C=0.0005050505;
ax=2.81977572474422*10^(-8);
bx=-0.000418283489244518;
xn=ax*lati^5+bx*lati^3+(514.633610723326-90^5*ax-90^3*bx)/90*lati;
by=-2.44045034942499*10^(-10);
cy=4.0096141899894*10^(-6);
yn=by*lati^6+cy*lati^4+(348.583307650417-715.2375...
-90^6*by-90^4*cy)/90^2*lati^2+715.3275;

rho=(yn^2+(xn-S)^2)/(2*(xn-x0));
deltan=asin(yn/rho);
delta=deltan/180*b1*(1-C*abs(long))*long;

%���x��y����
xi=S+rho*(1-cos(delta));
yi=rho*sin(delta);

if lati<-90 && lati>90
  xi=NaN;
end 
if long<-180 && long>180
    yi=NaN;
end
end

