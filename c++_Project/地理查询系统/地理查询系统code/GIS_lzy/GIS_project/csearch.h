#ifndef CSEARCH_H
#define CSEARCH_H
#include "cquadtreept.h"
#include "cindex.h"
#include "cgeopoint.h"

class CSearch
{
public:
    CSearch();

    CQuadTreept Quadtree;
    CIndex index;
    CChncity* GraphicsSearch(CRect wrect,CGeopoint pt,CCityList fulldataset);
};

#endif // CSEARCH_H
