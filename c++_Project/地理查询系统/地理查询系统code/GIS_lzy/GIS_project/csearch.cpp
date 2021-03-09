#include "csearch.h"
#include "iostream"
using namespace std;
CSearch::CSearch()
{

}

CChncity* CSearch::GraphicsSearch(CRect wrect,CGeopoint pt,CCityList fulldataset)
{
//    Quadtree.QuadTreeBuild(4,wrect);
//    Quadtree.QuadtreeBuild(fulldataset,wrect);
    quadnode_t* tempt=new quadnode_t();
    tempt=Quadtree.find(Quadtree.QuadTree->root,pt);
    if(tempt!=0x00)
    return tempt->lst_obj_cities;
    else
        return 0;
//    cout<<tempt->lst_obj_cities->name;
}
