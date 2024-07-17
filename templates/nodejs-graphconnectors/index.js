#! /usr/bin/env node

import createDebug from 'debug';
import fs from 'fs-extra';
import Handlebars from 'handlebars';
import inquirer from 'inquirer';
import { isText } from 'istextorbinary';
import path from 'path';
import { adjectives, animals, uniqueNamesGenerator } from 'unique-names-generator';
import url from 'url';
const debug = createDebug('create-graph-connector');
const __dirname = url.fileURLToPath(new URL('.', import.meta.url));

async function promptUser() {
  const projectName = process.argv[2];
  const randomName = uniqueNamesGenerator({
    dictionaries: [adjectives, animals],
    style: 'capital',
    separator: ' '
  });
  const randomNameSlug = randomName.toLowerCase().replace(/ /g, '-');
  const randomNameId = randomNameSlug.replace(/-/g, '').substring(0, 32);

  const answers = await inquirer.prompt([
    {
      type: 'input',
      name: 'projectName',
      message: 'Project name:',
      default: randomNameSlug,
      validate: value => value.length > 0,
      when: !projectName
    },
    {
      type: 'input',
      name: 'connectorName',
      default: randomName,
      message: 'Graph connector name:',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'connectorDescription',
      default: 'Imports data from Contoso app',
      message: 'Graph connector description:',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'connectionId',
      default: randomNameId,
      message: 'Connection ID (between 3 and 32 chars):',
      validate: value => value.length >= 3 && value.length <= 32 ? true : 'Connection ID must be between 3 and 32 characters'
    },
    {
      type: 'list',
      name: 'template',
      default: 'ts',
      message: 'Template',
      choices: [
        { name: 'TypeScript (basic)', value: 'ts' },
        { name: 'TypeScript (Teams Toolkit)', value: 'ts-ttk' }
      ],
      validate: value => value.length > 0
    }
  ])
    // suppress prompt error
    .catch(() => {
      process.exit(1);
    });
  answers.projectName = projectName || answers.projectName;
  return answers;
}

async function createProject(answers) {
  const templateDir = path.resolve(__dirname, 'templates', answers.template);
  const targetDir = path.resolve(process.cwd(), answers.projectName);

  debug(`Copying template from ${templateDir} to ${targetDir}`);
  await fs.copy(templateDir, targetDir);

  const filesAndFolders = await fs.readdir(targetDir, { recursive: true });
  for (const fileOrFolder of filesAndFolders) {
    const filePath = path.join(targetDir, fileOrFolder);
    const fileStats = await fs.stat(filePath);

    if (fileStats.isFile() && isText(filePath, fs.readFileSync(filePath))) {
      debug(`Templating file ${fileOrFolder}`);
      const fileContents = await fs.readFile(filePath, 'utf8');
      const renderedContents = Handlebars.compile(fileContents)(answers);
      await fs.writeFile(filePath, renderedContents);
    }
    else {
      debug(`${fileOrFolder} is either a folder or a binary. Skipping`);
    }
  }

  console.log('');
  console.log(`Project '${answers.projectName}' created successfully.`);
  console.log(`Follow template-specific instructions in the project's README.md file.`);
  console.log('');
}

async function main() {
  const answers = await promptUser();
  await createProject(answers);
}

main().catch(console.error);