using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class MainForm : Form
{
    private TextBox exprBox, textBox;
    private Button checkBtn;
    private Label msgLabel;
    private RichTextBox resultBox;
    private Parser parser = new Parser();
    private Searcher searcher = new Searcher();

    public MainForm()
    {
        Text = "Проверка регулярных выражений";
        ClientSize = new Size(640, 500);
        MinimumSize = new Size(500, 460);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 11);
        BackColor = Color.FromArgb(245, 247, 250);

        var header = new Label
        {
            Text = "  Помощник проверки регулярных выражений",
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 15),
            TextAlign = ContentAlignment.MiddleLeft
        };

        exprBox = AddInput("Выражение:", 75);
        textBox = AddInput("Строка:", 150);

        checkBtn = new Button
        {
            Text = "Проверить",
            Left = 20,
            Top = 230,
            Width = 160,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        checkBtn.FlatAppearance.BorderSize = 0;
        checkBtn.Click += Check;
        AcceptButton = checkBtn;

        msgLabel = new Label
        {
            Left = 20,
            Top = 285,
            Width = 600,
            Height = 40,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };

        resultBox = new RichTextBox
        {
            Left = 20,
            Top = 335,
            Width = 600,
            Height = 145,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            Font = new Font("Consolas", 14)
        };

        Controls.AddRange(new Control[] { header, checkBtn, msgLabel, resultBox });
    }

    // Создаёт подпись и поле ввода
    private TextBox AddInput(string title, int top)
    {
        var label = new Label { Text = title, Left = 20, Top = top, Width = 300, Height = 25 };
        var box = new TextBox
        {
            Left = 20,
            Top = top + 27,
            Width = 600,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        Controls.Add(label);
        Controls.Add(box);
        return box;
    }

    private void ShowMessage(string text, Color back, Color fore)
    {
        msgLabel.Text = text;
        msgLabel.BackColor = back;
        msgLabel.ForeColor = fore;
    }

    private void Warn(string text)
    {
        ShowMessage(text, Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27));
    }

    private void Check(object sender, EventArgs e)
    {
        resultBox.Clear();
        msgLabel.Text = "";
        msgLabel.BackColor = BackColor;
        string expr = exprBox.Text;
        string s = textBox.Text;

        if (expr == "") { Warn("Предупреждение: введена пустая строка вместо выражения!"); return; }
        if (s == "") { Warn("Предупреждение: введена пустая строка!"); return; }

        Node tree;
        try
        {
            tree = parser.Parse(expr);
        }
        catch (RegexParseException ex)
        {
            Warn("Предупреждение: введено НЕ регулярное выражение — " + ex.Message);
            return;
        }

        resultBox.Text = s;
        List<int[]> found = searcher.FindAll(tree, s);
        foreach (int[] f in found)
        {
            resultBox.Select(f[0], f[1]);
            resultBox.SelectionBackColor = Color.Gold;
        }
        resultBox.Select(0, 0);

        if (found.Count > 0)
            ShowMessage("Выражение регулярное. Найдено совпадений: " + found.Count,
                        Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52));
        else
            ShowMessage("Выражение регулярное, но в строке не найдено.",
                        Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14));
    }
}