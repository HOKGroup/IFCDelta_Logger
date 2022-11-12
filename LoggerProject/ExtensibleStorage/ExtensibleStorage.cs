using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitLogger
{
    internal class ExtensibleStorage : ExternalEventInfo
    {







        private Document doc { get; set; }


        List<string> _loggerValue = new List<string>();
        List<string> _projectInfoValue = new List<string>();
         public SchemaField? SchemaFieldToEdit { get; set; }


        public ExtensibleStorage(Document document, List<string> revitLoggerValue , List<string> projectLoggerValue , SchemaField? schemaFieldToEdit )
        {
            doc = document;
            _loggerValue = revitLoggerValue;
            _projectInfoValue = projectLoggerValue;
            SchemaFieldToEdit = schemaFieldToEdit;
        }







        // to collect schema by names
        public static Schema GetSchemaByName(string name)
        {
            // schema
            Schema schema = null;
            // list of schema in memory
            IList<Schema> schemas = Schema.ListSchemas();

            if (schemas != null && schemas.Count > 0)
            {
                // iterate schema list
                foreach (Schema s in schemas)
                {
                    if (s.SchemaName == name)
                    {
                        schema = s;
                        break;
                    }
                }
            }
            return schema;
        }

        public void CreateSchema()
        {

            try
            {

                Settings.Settings.SettingOnOff = false;
                Settings.Settings.NotesOnOff = false;
                Settings.Settings.SelectFromOnOff = false;

                var ProjectInformation = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation).FirstOrDefault();


                // transaction 
                Transaction t = new Transaction(doc);
                t.Start("Create Schema");

                // build schema
                SchemaBuilder schemaBuilder = new SchemaBuilder(new Guid("A796A94F-9E45-4E82-B716-503178B74446"));
                // access level
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
               
                // vendor id
                schemaBuilder.SetVendorId("Magnetar");
                // schema name
                schemaBuilder.SetSchemaName("Magnetar");

                // create a field to store a string
                //  FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField("ProjectName", typeof(string));
                schemaBuilder.AddArrayField("MagnetarProjectInfo", typeof(string));
                schemaBuilder.AddArrayField("MagnetarRevitLogger", typeof(string));



                // finish (register)
                Schema schema = schemaBuilder.Finish();


                // create entity (instance)
                Entity entity = new Entity(schema);

                // get field from schema
                Field fieldProjectInfo = schema.GetField("MagnetarProjectInfo");
                Field fieldRevitLogger = schema.GetField("MagnetarRevitLogger");

                // assign value/data



                IList<string> projectInfoValues = new List<string>() { "", "", "","","" };       // for ProjectName, ProjectNumber, ExternalProjectID, Model Name, Model Discipline  respectively
                IList<string> revitLoggerValues = new List<string>() { "", "", "","false" };       // for scope, demoLink, ProjectNote, Export IFC per Element respectively
                entity.Set(fieldProjectInfo, projectInfoValues);
                entity.Set(fieldRevitLogger, revitLoggerValues);





                // assign entity to element       
                ProjectInformation.SetEntity(entity);



                // close transaction
                t.Commit();
            }
            catch (Exception ex)
            {

                TaskDialog.Show("Error", ex.Message);
            }



        }


        public void InitializeSetting()
        {
            IList<string> projectInfoValues = GetFieldValue(SchemaField.MagnetarProjectInfo);
            if (projectInfoValues != null && projectInfoValues.Count != 0 )
            {
                Settings.Settings.ProjectName = projectInfoValues[0];
                Settings.Settings.ProjectNumber = projectInfoValues[1];
                Settings.Settings.externalProjectID = projectInfoValues[2];
                Settings.Settings.modelName = projectInfoValues[3];
                Settings.Settings.modelDiscipline = projectInfoValues[4];
            }


            IList<string> revitLoggerValues = GetFieldValue(SchemaField.MagnetarRevitLogger);
            if (revitLoggerValues != null && revitLoggerValues.Count != 0 )
            {
                //Settings.Settings.scope = revitLoggerValues[0];
                Settings.Settings.demoLink = revitLoggerValues[1];
                Settings.Settings.ProjectNote = revitLoggerValues[2];
                try
                {
                    //because we add this property lately may be the user open an file without this key is stored and it will throw exception
                    //so we srround it with try catch statement, in the catch statment we will set a new key with the default value
                    Settings.Settings.DeltaFileExport = revitLoggerValues[3].ToLower() =="true";

                }
                catch (Exception e)
                {
                    _loggerValue = new List<string>() {"", revitLoggerValues[1], revitLoggerValues[2], "false"};
                    SetFieldValue(SchemaField.MagnetarRevitLogger);
                }
            }
          



        }


        public  IList<string>  GetFieldValue  (SchemaField schemaField)
        {

            var errortries = 1;
            retry:
            IList<string>  value = new List<string>();
            try
            {

          
            var ProjectInformation = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation).FirstOrDefault();


            // get schema by name (custom method)
            Schema schema = GetSchemaByName("Magnetar");

            // get entity from schema (saved with the element) 

                if (schema != null)
                {
                    Entity getEntity = ProjectInformation.GetEntity(schema);
                    // get value of the field from entity
                    if (schemaField == SchemaField.MagnetarProjectInfo)
                    {
                        value = getEntity.Get<IList<string>>(schema.GetField("MagnetarProjectInfo"));

                    }
                    else
                    {
                        value = getEntity.Get<IList<string>>(schema.GetField("MagnetarRevitLogger"));
                    }


                }








            }
            catch (Exception ex )
            {
                
                CreateSchema();
                if (errortries ==1)
                {
                    errortries++;
                goto retry;
                }
                TaskDialog.Show("Error", ex.Message);
                

            }

            return value;


        }

        public void SetFieldValue(SchemaField schemaField )
        {
            var errortries = 1;
                Transaction t = new Transaction(doc);
        retry:
            try
            {
                t.Start("Edit Revit Logger Settings");

                var ProjectInformation = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation).FirstOrDefault();


                // get schema by name (custom method)
                Schema schema = GetSchemaByName("Magnetar");

                // get entity from schema (saved with the element) 
                Entity entity = ProjectInformation.GetEntity(schema);
                // set value of the field from entity

                if (schemaField == SchemaField.MagnetarProjectInfo || schemaField == SchemaField.both)
                {
                    entity.Set<IList<string>>(schema.GetField("MagnetarProjectInfo"), _projectInfoValue);
                }
                 if (schemaField == SchemaField.MagnetarRevitLogger || schemaField == SchemaField.both)
                {

                   
                    entity.Set<IList<string>>(schema.GetField("MagnetarRevitLogger"), _loggerValue);

                    //old code 
                    //var oldValues  = GetFieldValue(SchemaField.MagnetarRevitLogger);
                    //var oldScope = oldValues[0];
                    //var oldDemoLink = oldValues[1];
                    //var oldProjectNote = oldValues[2];

                    //// we have pass the project note from sperate window of scope and demoLink so if we have stored project note,
                    //// we don't want to overwrite scope and DemoLink with empty values
                    //if (oldScope != "" && _loggerValue[0] == "")
                    //{
                    //    List<string> newValues = new List<string>() { oldScope, oldDemoLink, _loggerValue[2] };

                    //    entity.Set<IList<string>>(schema.GetField("MagnetarRevitLogger"), newValues);

                    //}

                    ////in the same way if we have a project note already we want to not be overwriiter by empty value when we pass scope and demoLink
                    //else if (_loggerValue[0] != "" && oldProjectNote != "" && _loggerValue[2]=="")
                    //{

                    //    List<string> newValues = new List<string>() { _loggerValue[0], _loggerValue[1], oldProjectNote };

                    //    entity.Set<IList<string>>(schema.GetField("MagnetarRevitLogger"), newValues);
                    //}
                    
                    //// finally if the intial values is found there is no issue to update all the fields 
                    //else if (oldScope == "" && oldProjectNote == "" )
                    //{
                    //    entity.Set<IList<string>>(schema.GetField("MagnetarRevitLogger"), _loggerValue);
                    //}
                    //else
                    //{
                    //    entity.Set<IList<string>>(schema.GetField("MagnetarRevitLogger"), _loggerValue);

                    //}




                   
                }


                ProjectInformation.SetEntity(entity);


                t.Commit();

            }
            catch (Exception ex)
            {
                t.RollBack();
                CreateSchema();
                if (errortries == 1)
                {
                    errortries++;
                    goto retry;
                }
                TaskDialog.Show("Error", ex.Message);

            }

        }










        public override void Execute()
        {
          
            if (SchemaFieldToEdit!=null )
            {
                SetFieldValue(SchemaFieldToEdit.Value);
                InitializeSetting();
            }
            else
            {
                CreateSchema();
            }



        }

       
    }

    public enum SchemaField
    {
        MagnetarProjectInfo,
        MagnetarRevitLogger,
        both
    }
}
