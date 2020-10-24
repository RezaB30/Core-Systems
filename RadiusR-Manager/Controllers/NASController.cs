using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Mikrotik.Extentions;
using RezaB.Networking.IP;
using NLog;
using RezaB.Web.CustomAttributes;
using RezaB.Networking;

namespace RadiusR_Manager.Controllers
{
    [AuthorizePermission(Permissions = "NAS")]
    public class NASController : BaseController
    {
        RadiusREntities db = new RadiusREntities();
        private static Logger logger = LogManager.GetLogger("router_api_errors");
        // GET: NAS
        public ActionResult Index(int? page)
        {
            var viewResults = db.NAS.OrderBy(nas => nas.ID).Select(nas => new NASViewModel()
            {
                ID = nas.ID,
                BackboneNASID = nas.BackboneNASID,
                BackboneNAS = nas.BackboneNAS,
                IP = nas.IP,
                NATType = nas.NATType,
                Name = nas.Name,
                RadiusIncomingPort = nas.RadiusIncomingPort.ToString(),
                Secret = nas.Secret,
                NASType = nas.TypeID,
                ApiUsername = nas.ApiUsername,
                ApiPassword = nas.ApiPassword,
                ApiPort = nas.ApiPort.ToString(),
                Disabled = nas.Disabled,
                NASVerticalIPMaps = nas.NASVerticalIPMaps.Select(ipMap => new NASVerticalIPMapViewModel()
                {
                    ID = ipMap.ID,
                    LocalIPStart = ipMap.LocalIPStart,
                    LocalIPEnd = ipMap.LocalIPEnd,
                    RealIPStart = ipMap.RealIPStart,
                    RealIPEnd = ipMap.RealIPEnd,
                    _PortCount = ipMap.PortCount
                }),
                NASNetmaps = nas.NASNetmaps.Select(netmap => new NASNetmapViewModel()
                {
                    ID = netmap.ID,
                    LocalIPSubnet = netmap.LocalIPSubnet,
                    RealIPSubnet = netmap.RealIPSubnet,
                    _PortCount = netmap.PortCount
                })
            });
            SetupPages(page, ref viewResults);
            return View(viewResults);
        }

        // GET: NAS/Add
        public ActionResult Add()
        {
            //ViewBag.NASTypes = new SelectList(db.NASTypes.ToList(), "TypeID", "Name");
            ViewBag.NASes = new SelectList(db.NAS.ToList(), "ID", "Name");
            //var nas = new NASViewModel()
            //{
            //    //NASVerticalIPMaps = new List<NASVerticalIPMapViewModel>(),
            //    //NASNetmaps = new List<NASNetmapViewModel>()
            //};

            return View(/*nas*/);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/Add
        public ActionResult Add([Bind(Include = "BackboneNASID,Name,IP,RadiusIncomingPort,Secret,ApiUsername,ApiPassword,ApiPort,NASType,NATType")]NASViewModel nasModel)
        {
            if (ModelState.IsValid)
            {
                var nas = new NAS()
                {
                    BackboneNASID = nasModel.BackboneNASID,
                    Name = nasModel.Name,
                    IP = nasModel.IP,
                    RadiusIncomingPort = int.Parse(nasModel.RadiusIncomingPort),
                    Secret = nasModel.Secret,
                    TypeID = nasModel.NASType.Value,
                    NATType = nasModel.NATType,
                    ApiUsername = nasModel.ApiUsername,
                    ApiPassword = nasModel.ApiPassword,
                    ApiPort = int.Parse(nasModel.ApiPort),
                    Disabled = false
                };

                db.NAS.Add(nas);
                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            //if (nasModel.NASVerticalIPMaps == null)
            //    nasModel.NASVerticalIPMaps = new List<NASVerticalIPMapViewModel>();
            //if (nasModel.NASNetmaps == null)
            //    nasModel.NASNetmaps = new List<NASNetmapViewModel>();
            //ViewBag.NASTypes = new SelectList(db.NASTypes.ToList(), "TypeID", "Name", nasModel.TypeID);
            ViewBag.NASes = new SelectList(db.NAS.ToList(), "ID", "Name");
            return View(nasModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/Remove
        public ActionResult Remove(long ID)
        {
            var dbNas = db.NAS.Find(ID);
            if (dbNas != null)
            {
                db.NASVerticalIPMaps.RemoveRange(dbNas.NASVerticalIPMaps);
                db.NASNetmaps.RemoveRange(dbNas.NASNetmaps);
                db.NASExpiredPools.RemoveRange(dbNas.NASExpiredPools);
                db.NAS.Remove(dbNas);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        [HttpGet]
        // GET: NAS/Edit
        public ActionResult Edit(long id)
        {
            var dbNas = db.NAS.Find(id);
            if (dbNas == null)
            {
                return RedirectToAction("Index", new { errorMessage = 16 });
            }

            var nas = new NASViewModel()
            {
                ID = dbNas.ID,
                BackboneNASID = dbNas.BackboneNASID,
                IP = dbNas.IP,
                Name = dbNas.Name,
                ApiPassword = dbNas.ApiPassword,
                ApiUsername = dbNas.ApiUsername,
                NASType = dbNas.TypeID,
                NATType = dbNas.NATType,
                RadiusIncomingPort = dbNas.RadiusIncomingPort.ToString(),
                Secret = dbNas.Secret,
                ApiPort = dbNas.ApiPort.ToString()
            };

            //ViewBag.NASTypes = new SelectList(db.NASTypes.ToList(), "TypeID", "Name", nas.TypeID);
            ViewBag.NASes = new SelectList(db.NAS.Where(n => n.ID != dbNas.ID).ToList(), "ID", "Name", dbNas.BackboneNASID);
            return View(nas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/Edit
        public ActionResult Edit(long id, [Bind(Include = "BackboneNASID,Name,IP,RadiusIncomingPort,Secret,ApiUsername,ApiPassword,ApiPort,NASType,NATType")]NASViewModel nas)
        {
            var dbNas = db.NAS.Find(id);
            if (dbNas == null)
            {
                return RedirectToAction("Index", new { errorMessage = 16 });
            }

            if (ModelState.IsValid)
            {
                // changes in NAT rules ------------
                if (dbNas.NATType != nas.NATType)
                {
                    var router = new MikrotikRouter(new MikrotikApiCredentials(dbNas.IP, dbNas.ApiPort, dbNas.ApiUsername, dbNas.ApiPassword), 10000);
                    if (!router.ConfirmVerticalNAT(true) || !router.ClearActiveVerticalNATs())
                    {
                        ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                        logger.Error(router.ExceptionLog);
                    }
                    if (!router.ConfirmNetmapChanges(true) || !router.ClearActiveNetmaps())
                    {
                        ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                        logger.Error(router.ExceptionLog);
                    }
                    if (ModelState.IsValid)
                    {
                        db.NASVerticalIPMaps.RemoveRange(dbNas.NASVerticalIPMaps);
                        db.NASNetmaps.RemoveRange(dbNas.NASNetmaps);
                    }
                }
                //----------------------------------
                if (ModelState.IsValid)
                {
                    dbNas.BackboneNASID = nas.BackboneNASID;
                    dbNas.IP = nas.IP;
                    dbNas.Name = nas.Name;
                    dbNas.TypeID = nas.NASType.Value;
                    dbNas.RadiusIncomingPort = int.Parse(nas.RadiusIncomingPort);
                    dbNas.Secret = nas.Secret;
                    dbNas.ApiUsername = nas.ApiUsername;
                    dbNas.ApiPassword = nas.ApiPassword;
                    dbNas.ApiPort = int.Parse(nas.ApiPort);
                    dbNas.NATType = nas.NATType;

                    db.SaveChanges();

                    return RedirectToAction("Index", new { errorMessage = 0 });
                }
            }

            //ViewBag.NASTypes = new SelectList(db.NASTypes.ToList(), "TypeID", "Name", nas.TypeID);
            ViewBag.NASes = new SelectList(db.NAS.Where(n => n.ID != dbNas.ID).ToList(), "ID", "Name", dbNas.BackboneNASID);
            return View(nas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/Toggle
        public ActionResult Toggle(int id)
        {
            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            dbNAS.Disabled = !dbNAS.Disabled;
            db.SaveChanges();

            return RedirectToAction("Index", new { errorMessage = 0 });
        }

        // GET: NAS/EditNAT
        public ActionResult EditNAT(int id)
        {
            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            switch (dbNAS.NATType)
            {
                case (int)NATType.Horizontal:
                    return RedirectToAction("EditNetmap", new { id = dbNAS.ID });
                case (int)NATType.Vertical:
                case (int)NATType.VerticalDSL:
                    return RedirectToAction("EditVerticalIPMaps", new { id = dbNAS.ID });
                default:
                    return RedirectToAction("Index", new { errorMessage = 9 });
            }
        }

        [HttpGet]
        // GET: NAS/EditIPPools
        public ActionResult EditVerticalIPMaps(int id)
        {
            var validNATTypes = new List<NATType>()
            {
                NATType.Vertical,
                NATType.VerticalDSL
            };

            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            if (!validNATTypes.Contains((NATType)dbNAS.NATType))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var VerticalIPMaps = new VerticalIPMapListViewModel()
            {
                NASName = dbNAS.Name,
                NASVerticalIPMaps = dbNAS.NASVerticalIPMaps.Select(ipMap => new NASVerticalIPMapViewModel()
                {
                    ID = ipMap.ID,
                    LocalIPStart = ipMap.LocalIPStart,
                    LocalIPEnd = ipMap.LocalIPEnd,
                    RealIPStart = ipMap.RealIPStart,
                    RealIPEnd = ipMap.RealIPEnd,
                    _PortCount = ipMap.PortCount
                })
            };

            return View(VerticalIPMaps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/EditIPPools
        public ActionResult EditVerticalIPMaps(int id, [Bind(Include = "NASVerticalIPMaps")] VerticalIPMapListViewModel verticalIPMaps)
        {
            var validNATTypes = new List<NATType>()
            {
                NATType.Vertical,
                NATType.VerticalDSL
            };

            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            if (!validNATTypes.Contains((NATType)dbNAS.NATType))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                // validate IP pools--------------------------------
                if (verticalIPMaps.NASVerticalIPMaps == null)
                    verticalIPMaps.NASVerticalIPMaps = new List<NASVerticalIPMapViewModel>();
                for (int i = 0; i < verticalIPMaps.NASVerticalIPMaps.Count(); i++)
                {
                    {
                        var lower = IPTools.GetUIntValue(verticalIPMaps.NASVerticalIPMaps.ToArray()[i].LocalIPStart);
                        var higher = IPTools.GetUIntValue(verticalIPMaps.NASVerticalIPMaps.ToArray()[i].LocalIPEnd);
                        if (lower > higher)
                        {
                            ModelState.AddModelError("NASVerticalIPMaps[" + i + "].Set", RadiusR.Localization.Validation.Common.InvalidIPRange);
                        }
                        else
                        {
                            lower = IPTools.GetUIntValue(verticalIPMaps.NASVerticalIPMaps.ToArray()[i].RealIPStart);
                            higher = IPTools.GetUIntValue(verticalIPMaps.NASVerticalIPMaps.ToArray()[i].RealIPEnd);
                            if (lower > higher)
                            {
                                ModelState.AddModelError("NASVerticalIPMaps[" + i + "].Set", RadiusR.Localization.Validation.Common.InvalidIPRange);
                            }
                        }
                    }
                }
                //---------------------------------------------------
                if (ModelState.IsValid)
                {
                    //// upload to router ----------
                    //var errorOccured = false;
                    //var router = new MikrotikRouter(new MikrotikApiCredentials(dbNAS.IP, dbNAS.ApiPort, dbNAS.ApiUsername, dbNAS.ApiPassword), 30000);
                    //if (!router.ConfirmVerticalNAT())
                    //{
                    //    ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //    errorOccured = true;
                    //    logger.Error(router.ExceptionLog);
                    //}
                    //var verticalIPMapCluster = verticalIPMaps.NASVerticalIPMaps.Select(map => new VerticalIPMap()
                    //{
                    //    LocalIPStart = map.LocalIPStart,
                    //    LocalIPEnd = map.LocalIPEnd,
                    //    RealIPStart = map.RealIPStart,
                    //    RealIPEnd = map.RealIPEnd,
                    //    PortCount = Convert.ToUInt16(map._PortCount)
                    //});
                    //if (!errorOccured && !router.InsertVerticalNATRuleCluster(verticalIPMapCluster))
                    //{
                    //    ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //    errorOccured = true;
                    //    logger.Error(router.ExceptionLog);
                    //}
                    //if (errorOccured)
                    //{
                    //    router.ReverseVerticalNATChanges();
                    //}
                    //else
                    //{
                    //    if (!router.ClearActiveVerticalNATs() || !router.ConfirmVerticalNAT(true))
                    //    {
                    //        ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //        errorOccured = true;
                    //        logger.Error(router.ExceptionLog);
                    //    }
                    //}
                    ////----------------------------
                    if (ModelState.IsValid)
                    {
                        var oldVerticalIPMaps = dbNAS.NASVerticalIPMaps;
                        db.NASVerticalIPMaps.RemoveRange(oldVerticalIPMaps);
                        dbNAS.NASVerticalIPMaps = verticalIPMaps.NASVerticalIPMaps.Select(ipMap => new NASVerticalIPMap()
                        {
                            LocalIPStart = ipMap.LocalIPStart,
                            LocalIPEnd = ipMap.LocalIPEnd,
                            RealIPStart = ipMap.RealIPStart,
                            RealIPEnd = ipMap.RealIPEnd,
                            PortCount = ipMap._PortCount.Value
                        }).ToList();

                        db.SaveChanges();
                        return RedirectToAction("Index", new { errorMessage = 0 });
                    }
                }
            }
            verticalIPMaps.NASName = dbNAS.Name;
            return View(verticalIPMaps);
        }

        [HttpGet]
        // GET: NAS/EditNetmap
        public ActionResult EditNetmap(int id)
        {
            var validNATTypes = new List<NATType>()
            {
                NATType.Horizontal
            };

            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            if (!validNATTypes.Contains((NATType)dbNAS.NATType))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            var Netmaps = new NetmapListViewModel()
            {
                NASName = dbNAS.Name,
                NASNetmaps = dbNAS.NASNetmaps.Select(netmap => new NASNetmapViewModel()
                {
                    ID = netmap.ID,
                    LocalIPSubnet = netmap.LocalIPSubnet,
                    RealIPSubnet = netmap.RealIPSubnet,
                    _PortCount = netmap.PortCount,
                    PreserveLastByte = netmap.PreserveLastByte
                })
            };

            return View(Netmaps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/EditNetmap
        public ActionResult EditNetmap(int id, [Bind(Include = "NASNetmaps")]NetmapListViewModel netmaps)
        {
            var validNATTypes = new List<NATType>()
            {
                NATType.Horizontal
            };

            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }
            if (!validNATTypes.Contains((NATType)dbNAS.NATType))
            {
                return RedirectToAction("Index", new { errorMessage = 9 });
            }

            if (ModelState.IsValid)
            {
                //validate netmaps-----------------------------------
                if (netmaps.NASNetmaps == null)
                    netmaps.NASNetmaps = new List<NASNetmapViewModel>();
                for (int i = 0; i < netmaps.NASNetmaps.Count(); i++)
                {
                    var localSubnet = netmaps.NASNetmaps.ToArray()[i].LocalIPSubnet.Split('/')[1];
                    var realSubnet = netmaps.NASNetmaps.ToArray()[i].RealIPSubnet.Split('/')[1];
                    if (localSubnet != realSubnet)
                    {
                        ModelState.AddModelError("NASNetmaps[" + i + "].Set", RadiusR.Localization.Validation.Common.InvalidNetmap);
                    }
                }
                //---------------------------------------------------
                if (ModelState.IsValid)
                {
                    //// upload to router ----------
                    //var errorOccured = false;
                    //var router = new MikrotikRouter(new MikrotikApiCredentials(dbNAS.IP, dbNAS.ApiPort, dbNAS.ApiUsername, dbNAS.ApiPassword), 30000);
                    //if (!router.ConfirmNetmapChanges())
                    //{
                    //    ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //    errorOccured = true;
                    //    logger.Error(router.ExceptionLog);
                    //}
                    //foreach (var netmapCluster in netmaps.NASNetmaps)
                    //{
                    //    if (!errorOccured && !router.InsertNetmapCluster(netmapCluster.LocalIPSubnet, netmapCluster.RealIPSubnet, netmapCluster._PortCount.Value, netmapCluster.PreserveLastByte))
                    //    {
                    //        ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //        errorOccured = true;
                    //        logger.Error(router.ExceptionLog);
                    //    }
                    //}
                    //if (errorOccured)
                    //{
                    //    router.ReverseChanges();
                    //}
                    //else
                    //{
                    //    if (!router.ClearActiveNetmaps() || !router.ConfirmNetmapChanges(true))
                    //    {
                    //        ModelState.AddModelError("Router", RadiusR.Localization.Pages.ErrorMessages._19);
                    //        errorOccured = true;
                    //        logger.Error(router.ExceptionLog);
                    //    }
                    //}
                    ////----------------------------
                    if (ModelState.IsValid)
                    {
                        var oldNetmaps = dbNAS.NASNetmaps;
                        db.NASNetmaps.RemoveRange(oldNetmaps);
                        dbNAS.NASNetmaps = netmaps.NASNetmaps.Select(netmap => new NASNetmap()
                        {
                            LocalIPSubnet = netmap.LocalIPSubnet,
                            RealIPSubnet = netmap.RealIPSubnet,
                            PortCount = netmap._PortCount.Value,
                            PreserveLastByte = netmap.PreserveLastByte
                        }).ToList();

                        db.SaveChanges();
                        return RedirectToAction("Index", new { errorMessage = 0 });
                    }
                }
            }

            netmaps.NASName = dbNAS.Name;
            return View(netmaps);
        }

        [HttpGet]
        // GET: NAS/EditExpiredPools
        public ActionResult EditExpiredPools(int id)
        {
            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }

            var viewResults = new ExpiredPoolListViewModel()
            {
                NASName = dbNAS.Name,
                ExpiredPools = dbNAS.NASExpiredPools.Select(nep => new ExpiredPoolViewModel()
                {
                    ID = nep.ID,
                    ExpiredPoolName = nep.PoolName,
                    LocalIPSubnet = nep.LocalIPSubnet
                })
            };

            return View(viewResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: NAS/EditExpiredPools
        public ActionResult EditExpiredPools(int id, ExpiredPoolListViewModel expiredPoolList)
        {
            var dbNAS = db.NAS.Find(id);
            if (dbNAS == null)
            {
                return RedirectToAction("Index", new { errorMessage = 20 });
            }

            if (ModelState.IsValid)
            {
                db.NASExpiredPools.RemoveRange(db.NASExpiredPools.Where(nep => nep.NASID == dbNAS.ID));
                db.NASExpiredPools.AddRange(expiredPoolList.ExpiredPools.Select(epl => new NASExpiredPool()
                {
                    NASID = dbNAS.ID,
                    PoolName = epl.ExpiredPoolName,
                    LocalIPSubnet = epl.LocalIPSubnet
                }));

                db.SaveChanges();

                return RedirectToAction("Index", new { errorMessage = 0 });
            }

            expiredPoolList.NASName = dbNAS.Name;
            return View(expiredPoolList);
        }
    }
}