using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB;
using MongoDB.Bson;
namespace TestMongoDb
{
    

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private MongoCollection GetMongoDB()
        {
            string conn = "mongodb://localhost";
            string database = "demoBase";
            string collection = "demoCollection";
            MongoServer mongodb = MongoServer.Create(conn);
            MongoDatabase mongoDataBase = mongodb.GetDatabase(database);
            MongoCollection mongoCollection = mongoDataBase.GetCollection(collection);
            mongodb.Connect();
            return mongoCollection;
        }

        private void button1_Click(object sender, EventArgs e)
        {
              var collection = GetMongoDB();
            var t = new { Name = textBox1.Text, Sex =  r1.Checked ? "男" : "女" };
            collection.Insert(t);
            Student s=new Student{Name=textBox1.Text+"_Object",Sex=t.Sex};
            collection.Insert(s);
            BsonDocument doc = new BsonDocument();
            doc.Add("Name", textBox1.Text + "_Bson");
            doc.Add("Sex", t.Sex);
            collection.Insert(doc);
            BindData(0,0);

        }

        private void BindData(int frm,int to)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("ObjectId", "ObjectId");
            dataGridView1.Columns.Add("Name", "Name");
            dataGridView1.Columns.Add("Sex", "Sex");
            var collection = GetMongoDB();


            QueryDocument query = new QueryDocument();
           // query.Add("$gt", "男");
           // query.Add("Uid", "Sex");
            FieldsDocument field = new FieldsDocument();
            field.Add("Name", 1);
            field.Add("Sex", 1);
            SortByDocument sort = new SortByDocument();
            sort.Add("Name", -1); //-1 desc
            MongoCursor<Student> cursor ;
            if(frm>0 && to >0 && frm<to)
            cursor = collection.FindAs<Student>(query).SetFields(field).SetSortOrder(sort).SetSkip(frm).SetLimit(to-frm);
            else 
            cursor = collection.FindAs<Student>(query).SetFields(field).SetSortOrder(sort);

           // MongoCursor<Student> cursor  =collection.FindAllAs<Student>();
         
           // List<Student> lst = new List<Student>();
            foreach (Student s in cursor)
            {
              //  lst.Add(s);
                dataGridView1.Rows.Add(new object[] {s._id,s.Name,s.Sex });

            }
          
          


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindData(0,0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int frm=0, to=0;
            int.TryParse(t1.Text, out frm);
            int.TryParse(t2.Text, out to);
            BindData(frm, to);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var collection =GetMongoDB();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                
                QueryDocument query=new QueryDocument();
                query.Add("$eq",dr.Cells["ObjectId"].Value.ToBson());
                query.Add("Uid","ObjectId");
                UpdateDocument update=new UpdateDocument();
                update.Add("Name",dr.Cells["Name"].Value.ToBson());
                update.Add("Sex",dr.Cells["Sex"].Value.ToBson());
                collection.Update(query, update);

            }
        }

    }
}
