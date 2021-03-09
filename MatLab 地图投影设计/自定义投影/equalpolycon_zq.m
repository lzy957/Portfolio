function varargout = equalpolycon_zq(varargin)
%POLYCON  Polyconic Projection equal 
%
%  For this projection, each parallel has a curvature identical to its
%  curvature on a cone tangent at that latitude.  Since each parallel would
%  have its own cone, this is a "polyconic" projection. Scale is true along
%  the central meridian and along each parallel. This projection is free of
%  distortion only along the central meridian; distortion can be severe at
%  extreme longitudes.  This projection is neither conformal nor equal
%  area.
%
%  This projection was apparently originated about 1820 by Ferdinand
%  Rudolph Hassler.  It is also known as the American Polyconic and the
%  Ordinary Polyconic projections.
%
%  See also POLYCONSTD.

% Copyright 1996-2011 The MathWorks, Inc.

mproj.default = @equalpolyconDefault;
mproj.forward = @equalpolyconFwd;
mproj.inverse = @equalpolyconInv;
mproj.auxiliaryLatitudeType = 'rectifying';
mproj.classCode = 'Poly';

varargout = applyProjection(mproj, varargin{:});

%--------------------------------------------------------------------------

function mstruct = equalpolyconDefault(mstruct)

mstruct.mapparallels = [];
mstruct.nparallels   = 0;
mstruct.fixedorient  = [];
[mstruct.trimlat, mstruct.trimlon] ...
          = fromDegrees(mstruct.angleunits, [-90 90], [-180 180]);

%--------------------------------------------------------------------------

function [x, y] = equalpolyconFwd(mstruct, mu, lambda)

[a, e, epsilon, radius] = deriveParameters(mstruct);

% Back off of the +/- 180 degree points.  The inverse
% algorithm has trouble distinguishing points at -180 degrees
% when they are near the pole.

indx = find( abs(pi - abs(lambda)) <= epsilon);
if ~isempty(indx)
    lambda(indx) = lambda(indx) - sign(lambda(indx))*epsilon;
end

% Convert back to geodetic latitude -- both types are needed

phi = convertlat([a e], mu, 'rectifying', 'geodetic', 'nocheck');

% Back off of the +/- 90 degree points.  This allows
% the differentiation of longitudes at the poles of the transformed
% coordinate system.

indx = find(abs(pi/2 - abs(mu)) <= epsilon);
if ~isempty(indx)
    mu(indx)  = (pi/2 - epsilon) * sign(mu(indx));
    phi(indx) = (pi/2 - epsilon) * sign(phi(indx));
end

% Pick up NaN place holders

x = lambda;
y = mu;

% Eliminate singularities in transformations at 0 latitude.

indx1 = find(phi == 0);
% indx3 = find(phi==pi/2);
indx2 = find(phi ~= 0);
% indx2=find(indx2~=pi/2);
%

% 
% if (lambda>-pi/6) 
%     if(lambda<=pi)
%     lambda=lambda-pi*5/6;
%     end
% elseif (lambda>=-pi) 
%     if (lambda<=-pi/6)
%     lambda=lambda+7*pi/6;
%     end
% end

%change rad into degree
phi(indx2)=phi(indx2)/pi*180;
lambda(indx2)=lambda(indx2)/pi*180;
phi(indx1)=phi(indx1)/pi*180;
lambda(indx1)=lambda(indx1)/pi*180;
dc=90;

%中央经线上的x0
x0=(phi(indx2)+0.06683225*phi(indx2).^4)*radius;
%边缘经线上直角坐标 lambda=180 Xn、Yn

xn=x0+9.5493*phi(indx2);
yn=sqrt(112^2-xn.^2)+20;
rho=(yn.^2+(xn-x0).^2)./(2.*(xn-x0));
y0=132/(210*180)*pi*1.1*(1-0.10096478*tan(lambda(indx2)/5)).*lambda(indx2);
Ep=asin(yn./rho);
E=(Ep/180*1.1).*(1-0.10096478*tan(lambda(indx2)/5)).*lambda(indx2);

%求得x、y坐标


% Points at zero latitude

if ~isempty(indx1)
   % x(indx1) = yn1/(2*dc)*1.1.*(1-0.0005050505*lambda(indx1)).*lambda(indx1);
    y(indx1) = 0;
      x(indx1) = a * lambda(indx1);
end

% Points at non-zero latitude


% Points at non-zero latitude
if ~isempty(indx2)

    %极坐标
    
%     N = a ./ sqrt(1 - (e*sin(phi(indx2))).^2);
%     E = lambda(indx2) .* sin(phi(indx2));
%     x(indx2) = N .* cot(phi(indx2)) .* sin(E);
%     y(indx2) = x0+radius*mu(indx2) + ...
%       N .* cot(phi(indx2)) .* (1-cos(E));
  x(indx2)=rho.*sin(E);
 y(indx2)=rho+x0-rho.*sin(E);
end

%--------------------------------------------------------------------------

function [mu, lambda] = equalpolyconInv(mstruct, x, y)

[a, e, epsilon] = deriveParameters(mstruct);

%  Eliminate singularities in transformations at 0 latitude.

indx = find(y == 0);
if ~isempty(indx)
    y(indx) = epsilon;
end

A = y / a;
B = (x / a).^2 + A.^2;
convergence = 1E-10;
maxsteps = 100;
steps = 1;
phiNew = A;
converged = 0;

while ~converged && (steps <= maxsteps)
    steps = steps + 1;
    phiOld = phiNew;

    C = sqrt(1 - (e*sin(phiOld)).^2) .* tan(phiOld);

    Ma = (1 - e^2/4 - 3*e^4/64 - 5*e^6/256) * phiOld - ...
        (3*e^2/8 + 3*e^4/32 + 45*e^6/1024) * sin(2*phiOld) + ...
        (15*e^4/256 + 45*e^6/1024) * sin(4*phiOld) - ...
        (35*e^6/3072) * sin(6*phiOld);

    Mp = (1 - e^2/4 - 3*e^4/64 - 5*e^6/256) - ...
        2 * (3*e^2/8 + 3*e^4/32 + 45*e^6/1024) * cos(2*phiOld) + ...
        4 * (15*e^4/256 + 45*e^6/1024) * cos(4*phiOld) - ...
        6 * (35*e^6/3072) * cos(6*phiOld);

    num = A.*(C.*Ma + 1) - Ma - 0.5*(Ma.^2 + B).*C;
    fact1 = (e^2)*sin(2*phiOld) .* (Ma.^2 + B - 2*A.*Ma) ./ (4*C);
    fact2 = (A-Ma) .* (C.*Mp - 2./sin(2*phiOld)) - Mp;
    deltaPhi = num ./ (fact1 + fact2);

    if max(abs(deltaPhi(:))) <= convergence
        converged = 1;
    else
        phiNew = phiOld - deltaPhi;
    end
end

lambda = asin(x.*C/a) ./ sin(phiNew);

phiNew(indx) = 0;       %  Correct for y = 0 points

% Convert to rectifying latitude for use in rotatem
mu = convertlat([a e], phiNew, 'geodetic', 'rectifying', 'nocheck');

%--------------------------------------------------------------------------

function [a, e, epsilon, radius] = deriveParameters(mstruct)

epsilon = 5*epsm('radians');
[a, e, radius] = ellipsoidpropsRectifying(mstruct);
