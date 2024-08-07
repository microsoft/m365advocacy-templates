#! /usr/bin/env node

import createDebug from 'debug';
import fs from 'fs-extra';
import Handlebars from 'handlebars';
import inquirer from 'inquirer';
import { isText } from 'istextorbinary';
import path from 'path';
import { adjectives, animals, uniqueNamesGenerator } from 'unique-names-generator';
import url from 'url';
const debug = createDebug('create-teams-msgext-search');
const __dirname = url.fileURLToPath(new URL('.', import.meta.url));

async function promptUser() {
  const appInternalName = process.argv[2];
  const randomName = uniqueNamesGenerator({
    dictionaries: [adjectives, animals],
    style: 'capital',
    separator: ' '
  });
  const randomNameSlug = randomName.toLowerCase().replace(/ /g, '-');

  const answers = await inquirer.prompt([
    {
      type: 'input',
      name: 'appInternalName',
      message: `Internal name of the app: (${randomNameSlug})`,
      default: randomNameSlug,
      validate: value => value.length > 0,
      when: !appInternalName
    },
    {
      type: 'input',
      name: 'appDisplayName',
      message: 'Display name of the app: (Teams MsgExt Search)',
      default: 'Teams MsgExt Search',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'appDescriptionShort',
      message: 'Short description of the app (Teams message extension)',
      default: 'Teams message extension',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'appDescriptionFull',
      message: 'Full description of the app (Teams message extension with Search command)',
      default: 'Teams message extension with Search command',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'commandId',
      message: 'ID of the command (searchQuery)',
      default: 'searchQuery',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'commandDescription',
      message: 'Description of the command (Test command to run query)',
      default: 'Test command to run query',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'commandTitle',
      message: 'Title of the command (Search)',
      default: 'Search',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'parameterName',
      message: 'Name of the parameter (searchQuery)',
      default: 'searchQuery',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'parameterTitle',
      message: 'Title of the parameter (Search Query)',
      default: 'Search Query',
      validate: value => value.length > 0
    },
    {
      type: 'input',
      name: 'parameterDescription',
      message: 'Description of the parameter (Your search query)',
      default: 'Your search query',
      validate: value => value.length > 0
    },
    
  ])
    // suppress prompt error
    .catch(() => {
      process.exit(1);
    });
  answers.appInternalName = appInternalName || answers.appInternalName;
  return answers;
}

async function createProject(answers) {
  const templateDir = path.resolve(__dirname, 'templates', 'ts-ttk');
  const targetDir = path.resolve(process.cwd(), answers.appInternalName);

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

    if (fileOrFolder.endsWith('_gitignore')) {
      debug(`Renaming ${fileOrFolder} to .gitignore`);
      const newFilePath = path.join(targetDir, fileOrFolder.replace('_gitignore', '.gitignore'));
      await fs.move(filePath, newFilePath);
    }
  }

  console.log('');
  console.log(`Project '${answers.appInternalName}' created successfully.`);
  console.log(`Follow template-specific instructions in the project's README.md file.`);
  console.log('');
}

async function main() {
  const answers = await promptUser();
  await createProject(answers);
}

main().catch(console.error);