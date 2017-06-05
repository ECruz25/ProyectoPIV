using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;

using Proyecto_21351029.Models;
namespace Proyecto_21351029.Controllers
{
    public class CalendarController : Controller
    {
        public ActionResult Index()
        {
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
            var scheduler = new DHXScheduler(this);
            /*var cl = new LightboxSelect("type", "Type");
            select.AddOptions(new List<object>{
                new { key = 1, label = "Job" },
                new { key = 2, label = "Family" },
                new { key = 3, label = "Other" }
            });
            scheduler.Lightbox.Add(select);
            scheduler.Lightbox.AddDefaults();*/
            /*
             * It's possible to use different actions of the current controller
             *      var scheduler = new DHXScheduler(this);     
             *      scheduler.DataAction = "ActionName1";
             *      scheduler.SaveAction = "ActionName2";
             * 
             * Or to specify full paths
             *      var scheduler = new DHXScheduler();
             *      scheduler.DataAction = Url.Action("Data", "Calendar");
             *      scheduler.SaveAction = Url.Action("Save", "Calendar");
             */

            /*
             * The default codebase folder is ~/Scripts/dhtmlxScheduler. It can be overriden:
             *      scheduler.Codebase = Url.Content("~/customCodebaseFolder");
             */

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;

            return View(scheduler);
        }

        public ContentResult Data()
        {
            var data = new SchedulerAjaxData(new ProyectoEntities().Tutorials);
                    
            return (ContentResult)data;
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            var changedEvent = (Tutorial)DHXEventsHelper.Bind(typeof(Tutorial), actionValues);
            changedEvent.class_code = "1";
            changedEvent.tutor_code = "0";
            var data = new ProyectoEntities();
            /*try
            {*/
                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        data.Tutorials.Add(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        changedEvent = data.Tutorials.FirstOrDefault(ev => ev.id == action.SourceId);
                        data.Tutorials.Remove(changedEvent);
                        break;
                    default:// "update"                          
                        var EventToUpdate = data.Tutorials.SingleOrDefault(ev => ev.id == action.SourceId);
                        DHXEventsHelper.Update(EventToUpdate, changedEvent, new List<string>(){"id"});
                    //return RedirectToAction("Edit", "Tutorials", new int { "id" = changedEvent.id });
                    break;
                }
                data.SaveChanges();
                action.TargetId =   changedEvent.id;
            /*}
            catch
            {
                action.Type = DataActionTypes.Error;
            }*/
            return (ContentResult)new AjaxSaveResponse(action);
        }
    }
}

