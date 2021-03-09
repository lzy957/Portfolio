#include "cquadtreept.h"

CQuadTreept::CQuadTreept()
{
    this->QuadTree=new quadtree_t();
}

CQuadTreept::~CQuadTreept()
{
//    if(QuadTree!=0x00)
//    {
//        deletenode(QuadTree->root);
//        QuadTree->depth=0;
//    }
}


void CQuadTreept::QuadTreeBuild (int depth,CRect rect)
{
    this->QuadTree->depth = depth;

/*创建分支，root树的根，depth深度，rect根节点代表的矩形区域*/
    this->QuadCreateBranch (this->QuadTree->root, depth, rect );
}



void CQuadTreept::QuadCreateBranch (quadnode_t* &n,int depth,CRect rect )
{
    if ( depth )
    {
//        quadnode_t* tempt=new quadnode_t;
        n = new quadnode_t;    //开辟新节点
        n ->rect = rect; //将该节点所代表的矩形区域存储到该节点中
        CRect subrect[4];
        rect.Split(subrect, subrect+1, subrect+2, subrect+3);
        //将rect划成四份 rect[UR], rect[UL], rect[LL], rect[LR];

        /*创建各孩子分支*/
        this->QuadCreateBranch ( n->sub[UR], depth-1, subrect [UR] );
        this->QuadCreateBranch ( n->sub[UL], depth-1, subrect [UL] );
        this->QuadCreateBranch ( n->sub[LL], depth-1, subrect [LL] );
        this->QuadCreateBranch ( n->sub[LR], depth-1, subrect [LR] );
   }
   }

void CQuadTreept::QuadtreeBuild(CCityList fulldataset,CRect rect)
  {
    this->QuadTree->root=new quadnode_t();
    this->QuadTree->root->lst_obj_cities=new CChncity;
    this->QuadTree->root->lst_obj_cities=NULL;
    this->QuadTree->root->rect=rect;
     list<CChncity*>::iterator i;
//     this->QuadTree.root->lst_obj_cities=i;
     for (i=fulldataset.CityList.begin();i!=fulldataset.CityList.end();++i)      //遍历所有对象
    {
      this->QuadInsert(*i, this->QuadTree->root);//将i对象插入四叉树
    }
         //剔除多余的节点；       //执行完上面循环后，四叉树中可能有数据为空的叶子节点需要剔除
  }


void CQuadTreept::QuadInsert(CChncity* i,quadnode_t* &n)      //该函数插入后四叉树中的每个节点所存储的对象数量不是1就是0
  {
    CRect subrect[4];
    n->rect.Split(subrect, subrect+1, subrect+2, subrect+3);
    //将rect划成四份 rect[UR], rect[UL], rect[LL], rect[LR];

    CGeopoint pt;
    pt.x=i->x;
    pt.y=i->y;
    int quadrant=0;
    quadrant=n->rect.quadrant(pt);
//    int length=(n->rect.top-n->rect.bottom)/2;
//    int width=(n->rect.right-n->rect.left)/2;
     if(n->sub[0]!=nullptr||n->sub[1]!=nullptr||n->sub[2]!=nullptr||n->sub[3]!=nullptr)//（节点n有孩子）
     {
         if(quadrant!=-1)
         {if(n->sub[quadrant]==nullptr)
         {
             n->sub[quadrant]=new quadnode_t();
             n->sub[quadrant]->rect=subrect[quadrant];
         }
         this->QuadInsert(i,n->sub[quadrant]);
         }
//    通过划分区域判断i应该放置于n节点的哪一个孩子节点c；
    }
     else if((n->lst_obj_cities)!=NULL)  //（节点n存储了一个对象）
     {

//         this->QuadCreateBranch(n,1,n->rect);  //为n节点创建四个孩子；

          CGeopoint pcity;
          pcity.x=n->lst_obj_cities->x;
          pcity.y=n->lst_obj_cities->y;
          int quadrantcity=n->rect.quadrant(pcity);
          if(quadrantcity!=-1)
          {
              n->sub[quadrantcity]=new quadnode_t();
              n->sub[quadrantcity]->rect=subrect[quadrantcity];
              n->sub[quadrantcity]->lst_obj_cities=new CChncity;
              n->sub[quadrantcity]->lst_obj_cities=n->lst_obj_cities;
              n->lst_obj_cities=nullptr;
          }
          CGeopoint pi;
          pi.x=i->x;
          pi.y=i->y;
          int quadranti=n->rect.quadrant(pi);
          if(quadranti!=-1)
          {
              if(n->sub[quadranti]==nullptr)
              {
                  n->sub[quadranti]=new quadnode_t();
                  n->sub[quadranti]->rect=subrect[quadranti];
              }
              this->QuadInsert(i,n->sub[quadranti]);
          }

//    通过划分区域判断i应该放置于n节点的哪一个孩子节点c；
         }

    //将n节点中的对象移到它应该放置的孩子节点中；
    //通过划分区域判断i应该放置于n节点的哪一个孩子节点c；

     else if(n->lst_obj_cities==nullptr)//（n节点数据为空）
 {
         CChncity* temp=new CChncity;
         temp=i;
         n->lst_obj_cities=i;
         this->QuadTree->depth++;
//    n->lst_obj_cities.push_back(temp);//将i存储到节点n中；
 }
  }

quadnode_t* CQuadTreept::find (quadnode_t* &n,CGeopoint pt)
  {
    quadnode_t* temp=new quadnode_t();
    CRect recttemp;
    recttemp.left=pt.x-30000;
    recttemp.right=pt.x+30000;
    recttemp.top=pt.y+30000;
    recttemp.bottom=pt.y-30000;
    CGeopoint ptempt;
//    if(n->lst_obj_cities!=nullptr&&(n->lst_obj_cities->x==pt.x)&&(n->lst_obj_cities->y==pt.y))  //(n节点所存的对象位置为 pos所指的位置 )
    if(n->lst_obj_cities!=nullptr)  //(n节点所存的对象位置为 pos所指的位置 )
        {
            ptempt.x=n->lst_obj_cities->x;
            ptempt.y=n->lst_obj_cities->y;
            if(recttemp.contain(ptempt))
            return n;
         }
    int quadrant=n->rect.quadrant(pt);
    if(quadrant!=-1&&n->sub[quadrant]!=0x00)
        temp=find(n->sub[quadrant],pt);
    else
        return 0;
        return temp;

  }

void CQuadTreept::deletenode(quadnode_t *&n)
{
    for(int i=0;i<4;i++)
    {
        if(n->sub[i]!=0x00)
        {
            deletenode(n->sub[i]);
        }
    }
    if(n->sub[0]==0x00&&n->sub[1]==0x00&&n->sub[2]==0x00&&n->sub[3]==0x00&&n!=0x00)
    {
        if(n->lst_obj_cities!=0x00)
        {
            delete(n->lst_obj_cities);
            n->lst_obj_cities=0x00;
        }
        delete[]n;
        n=0x00;
    }
}
