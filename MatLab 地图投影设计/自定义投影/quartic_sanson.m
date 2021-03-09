function varargout = quartic_sanson(varargin)
%QUARTIC  Sanson Pseudocylindrical Projection
%programmed by ������
%
%  This is an equal area projection.  Scale is true along the Equator, and
%  is constant along any parallel and between any pair of parallels
%  equidistant from the Equator.  Distortion is severe near the outer
%  meridians at high latitudes, but less so than on the Sinusoidal
%  projection.  It is free of distortion along the Equator.  This
%  projection is not conformal or equidistant.
%
%  This projection was presented by Karl Siemon in 1937 and independently
%  by Oscar Sherman Adams in 1945.

% Copyright 1996-2007 The MathWorks, Inc.

mproj.default = @quarticDefault;
mproj.forward = @quarticFwd;
mproj.inverse = @quarticInv;
mproj.auxiliaryLatitudeType = 'authalic';
mproj.classCode = 'Pcyl';

varargout = applyProjection(mproj, varargin{:});

%--------------------------------------------------------------------------

function mstruct = quarticDefault(mstruct)

[mstruct.trimlat, mstruct.trimlon] ...
          = fromDegrees(mstruct.angleunits, [-90 90], [-180 180]);
mstruct.mapparallels = 0;
mstruct.nparallels   = 0;
mstruct.fixedorient  = [];

%--------------------------------------------------------------------------

function [x, y] = quarticFwd(mstruct, lat, lon)

radius = rsphere('authalic',mstruct.geoid);

   %���ĵĴ���

y = radius * lat;                 
x = radius * cos(lat).*lon;

%--------------------------------------------------------------------------

function [lat, lon] = quarticInv(mstruct, x, y)

radius = rsphere('authalic',mstruct.geoid);

lat = 2 * asin(y / (2*radius));
lon = (cos(lat/2) .* x) ./ (radius * cos(lat));
